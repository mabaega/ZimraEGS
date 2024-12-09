using Zimra.ApiClient.Models;
using Zimra.ApiClient.Enums;
using ZimraEGS.Models;
using System.Text;
using Zimra.ApiClient;

namespace ZimraEGS.Helpers
{
    public class ReceiptHelper
    {
        private RelayData relayData { get; }
        public ReceiptHelper(RelayData _relayData)
        {
            relayData = _relayData;
        }
        public Receipt GenerateZimraReceipt()
        {
            Receipt receipt = new Receipt();

            receipt.ReceiptType = relayData.ReceiptType;

            receipt.ReceiptCurrency = relayData.CurrencyCode ?? "ZWG";
            receipt.ReceiptCounter = relayData.BusinessReference.LastReceiptCounter + 1;

            //Handle Rejected Invoice
            if(relayData.InvReceiptGlobalNo == 0) //New Invoice
            {
                receipt.ReceiptGlobalNo = relayData.DeviceStatus.LastReceiptGlobalNo + 1; 
                receipt.ReceiptDate = DateTimeOffset.Now;
            }
            else
            {
                receipt.ReceiptGlobalNo = relayData.InvReceiptGlobalNo;
                receipt.ReceiptDate = relayData.InvReceiptDate ?? DateTimeOffset.Now;
            }

            receipt.InvoiceNo = receipt.ReceiptGlobalNo.ToString("D4");

            receipt.BuyerData = new Buyer
            {
                BuyerRegisterName = relayData.BuyerRegisterName,
                BuyerTradeName = relayData.BuyerTradeName,
                BuyerTIN = relayData.BuyerTIN,
                VatNumber = relayData.BuyerVatNumber,
                BuyerContacts = new Contacts
                {
                    Email = relayData.BuyerEmail,
                    PhoneNo = relayData.BuyerPhoneNo,
                },
                BuyerAddress = new AddressDto
                {
                    Province = relayData.BuyerProvince,
                    City = relayData.BuyerCity,
                    Street = relayData.BuyerStreet,
                    HouseNo = relayData.BuyerHouseNo,
                },
            };

            if (receipt.ReceiptType != ReceiptType.FiscalInvoice)
            {
                receipt.ReceiptNotes = relayData.ReceiptNotes ?? "-";
                receipt.CreditDebitNote = new CreditDebitNote
                {
                    ReceiptID = relayData.ReceiptCounterRef,
                    DeviceID = relayData.DeviceIDRef,
                    ReceiptGlobalNo = relayData.ReceiptGlobalNoRef,
                    FiscalDayNo = relayData.FiscalDayNoRef
                };
            }

            receipt.ReceiptLinesTaxInclusive = relayData.ManagerInvoice?.AmountsIncludeTax ?? false;

            receipt.ReceiptLines = GetReceiptLine();

            receipt.ReceiptTaxes = GetReceiptTaxs([.. receipt.ReceiptLines]);

            receipt.ReceiptPayments = GetPayments();

            receipt.ReceiptTotal = receipt.ReceiptTaxes.Sum(x => x.SalesAmountWithTax);

            receipt.ReceiptPrintForm = relayData.ReceiptPrintForm == "Receipt48" ? ReceiptPrintForm.Receipt48 : ReceiptPrintForm.InvoiceA4;

            string ReceiptTaxsHash = GetReceiptTaxsHash([.. receipt.ReceiptTaxes], receipt.ReceiptCurrency);

            string SourcesHash =
                ((relayData.CertificateInfo.DeviceID.ToString() +
                receipt.ReceiptType +
                receipt.ReceiptCurrency +
                receipt.ReceiptGlobalNo.ToString() +
                receipt.ReceiptDate.ToString("yyyy-MM-ddTHH:mm:ss") +
                (receipt.ReceiptTotal * 100).ToString() +
                ReceiptTaxsHash).ToUpper() +
                relayData.BusinessReference.LastReceiptHash);

            // Generate hash
            byte[] hashByte = Utilities.GenerateHash(SourcesHash);
            Console.WriteLine("Hash (Base64): " + Convert.ToBase64String(hashByte));

            // Generate signature
            byte[] signatureByte = CertificateHelper.GenerateSignature(Convert.ToBase64String(hashByte), relayData.CertificateInfo.PrivateKey);
            Console.WriteLine("Signature (Base64): " + Convert.ToBase64String(signatureByte));

            // Set signature data
            receipt.ReceiptDeviceSignature = new SignatureData()
            {
                Hash = hashByte,
                Signature = signatureByte ?? Encoding.UTF8.GetBytes("")
            };

            // Verify signature
            bool isValid = CertificateHelper.VerifySignature(
                receipt.ReceiptDeviceSignature.Hash,
                receipt.ReceiptDeviceSignature.Signature,
                relayData.CertificateInfo.DeviceCertificate);

            if (isValid)
            {
                Console.WriteLine("Signature is valid.");
            }
            else
            {
                Console.WriteLine("Signature is invalid.");
                Console.WriteLine("Hash (Base64): " + Convert.ToBase64String(receipt.ReceiptDeviceSignature.Hash));
                Console.WriteLine("Signature (Base64): " + Convert.ToBase64String(receipt.ReceiptDeviceSignature.Signature));
                Console.WriteLine("Certificate: " + relayData.CertificateInfo.DeviceCertificate);
            }


            return receipt;
        }

        public List<ReceiptLine> GetReceiptLine()
        {
            List<ReceiptLine> ls = new List<ReceiptLine>();
            ManagerInvoice mi = relayData.ManagerInvoice;

            bool isIncludeTax = mi.AmountsIncludeTax;

            int rw = 1;
            foreach (Line l in mi.Lines)
            {
                ReceiptLine rl = new ReceiptLine();

                string HsCode = string.Empty;
                if (l.CustomFields2 != null && l.CustomFields2.Strings.TryGetValue(ManagerCustomField.HsCodeGuid, out string value))
                {
                    HsCode = value;
                }

                double taxRate = l.TaxCode?.Rate ?? 0;
                double unitPrice = l.UnitPrice;

                rl.ReceiptLineType = Zimra.ApiClient.Enums.ReceiptLineType.Sale;
                rl.ReceiptLineNo = rw;
                rl.ReceiptLineName = GetFirstNonNullOrEmpty(l.LineDescription, l.Item.Name, l.Item.ItemName);
                rl.ReceiptLineHSCode = HsCode.PadLeft(8,'0');
                rl.ReceiptLineQuantity = Math.Round(l.Qty, 2);
                rl.ReceiptLinePrice = Math.Round(isIncludeTax ? unitPrice / (1 + (taxRate / 100)) : unitPrice, 4);

                if (relayData.ReceiptType == ReceiptType.DebitNote)
                {
                    rl.ReceiptLinePrice = -1 * rl.ReceiptLinePrice;
                }

                rl.ReceiptLineTotal = Math.Round(l.Qty * (double)rl.ReceiptLinePrice, 2);

                if (l.TaxCode != null && l.TaxCode.Name.Contains('|'))
                {
                    var taxcode = l.TaxCode.Name.Split("|");
                    rl.TaxCode = taxcode[2].Trim();
                    rl.TaxID = GetIntValue(taxcode[1].Trim());
                    rl.TaxPercent = rl.TaxID == 1 ? null: taxRate;
                }
                else
                {
                    rl.TaxCode = "A";
                    rl.TaxID = 1;
                    rl.TaxPercent = null;
                }

                ls.Add(rl);
                rw += 1;
            }

            foreach (Line l in mi.Lines)
            {
                if (l.DiscountAmount > 0)
                {
                    ReceiptLine rl = new ReceiptLine();

                    string HsCode = string.Empty;
                    if (l.CustomFields2 != null && l.CustomFields2.Strings.TryGetValue(ManagerCustomField.HsCodeGuid, out string value))
                    {
                        HsCode = value;
                    }

                    double taxRate = l.TaxCode?.Rate ?? 0;
                    double Qty = (l.Qty == 0) ? 1 : l.Qty;
                    double unitPrice = Math.Round(l.DiscountAmount / l.Qty, 4);

                    rl.ReceiptLineType = Zimra.ApiClient.Enums.ReceiptLineType.Discount;
                    rl.ReceiptLineNo = rw;
                    rl.ReceiptLineName = "Discount: " + GetFirstNonNullOrEmpty(l.LineDescription, l.Item.Name, l.Item.ItemName);
                    rl.ReceiptLineHSCode = HsCode;
                    rl.ReceiptLineQuantity = Math.Round(l.Qty, 2);
                    rl.ReceiptLinePrice = Math.Round(-1 * (isIncludeTax ? unitPrice / (1 + (taxRate / 100)) : unitPrice), 4);

                    if (relayData.ReceiptType == ReceiptType.DebitNote)
                    {
                        rl.ReceiptLinePrice = -1 * rl.ReceiptLinePrice;
                    }

                    rl.ReceiptLineTotal = Math.Round(l.Qty * (double)rl.ReceiptLinePrice, 2);

                    if (l.TaxCode != null && l.TaxCode.Name.Contains('|'))
                    {
                        var taxcode = l.TaxCode.Name.Split('|');
                        rl.TaxCode = taxcode[2].Trim();
                        rl.TaxID = GetIntValue(taxcode[1].Trim());
                        rl.TaxPercent = rl.TaxID == 1 ? null : taxRate;
                    }
                    else
                    {
                        rl.TaxCode = "A";
                        rl.TaxID = 1;
                        rl.TaxPercent = null;
                    }

                    ls.Add(rl);
                    rw += 1;
                }
            }

            return ls;
        }

        private static string GetFirstNonNullOrEmpty(params string[] values)
        {
            foreach (var value in values)
            {
                if (!string.IsNullOrEmpty(value?.Trim()))
                {
                    return value;
                }
            }
            return string.Empty;
        }
        private int GetIntValue(string numString)
        {
            return int.TryParse(numString, out int value) ? value : 0;
        }
        private List<ReceiptTax> GetReceiptTaxs(List<ReceiptLine> receiptLines)
        {
            List<ReceiptTax> ls = new List<ReceiptTax>();

            foreach (ReceiptLine ln in receiptLines)
            {
                ReceiptTax receiptTax = new ReceiptTax();
                receiptTax.TaxCode = ln.TaxCode;
                receiptTax.TaxID = ln.TaxID;
                if (ln.TaxPercent != null) { receiptTax.TaxPercent = ln.TaxPercent; }

                var tx = (double)(ln.TaxPercent == null ? 0 : ln.TaxPercent);
                double tpersen = tx / 100;

                receiptTax.TaxAmount = Math.Round(ln.ReceiptLineTotal * tpersen, 2);
                receiptTax.SalesAmountWithTax = ln.ReceiptLineTotal + receiptTax.TaxAmount;

                ls.Add(receiptTax);
            }

            // Group by TaxCode, TaxID, TaxPercent and sum TaxAmount and SalesAmountWithTax
            var groupedResult = ls.GroupBy(x => new { x.TaxCode, x.TaxID, x.TaxPercent })
                                  .Select(g => new ReceiptTax
                                  {
                                      TaxCode = g.Key.TaxCode,
                                      TaxID = g.Key.TaxID,
                                      TaxPercent = g.Key.TaxPercent,
                                      TaxAmount = g.Sum(x => x.TaxAmount),
                                      SalesAmountWithTax = g.Sum(x => x.SalesAmountWithTax)
                                  }).OrderBy(o => o.TaxID).ToList();

            return groupedResult;
        }

        private List<Payment> GetPayments()
        {
            List<Payment> ls = new List<Payment>();

            if (relayData.PaymentAmount1 > 0 && relayData.PaymentType1 != null)
            {
                if (int.TryParse(relayData.PaymentType1.Trim()[..1], out int t))
                {
                    ls.Add(new Payment
                    {
                        MoneyTypeCode = (MoneyType)t,
                        PaymentAmount = relayData.PaymentAmount1
                    });
                }
            }

            if (relayData.PaymentAmount2 > 0 && relayData.PaymentType2 != null)
            {
                if (int.TryParse(relayData.PaymentType2.Trim()[..1], out int t))
                {
                    ls.Add(new Payment
                    {
                        MoneyTypeCode = (MoneyType)t,
                        PaymentAmount = relayData.PaymentAmount2
                    });
                }
            }

            return ls;
        }

        private string GetReceiptTaxsHash(List<ReceiptTax> receiptTaxs, string receiptCurrency)
        {
            string sHash = string.Empty;

            foreach (ReceiptTax tx in receiptTaxs)
            {
                string scode = "";
                string tcode = receiptCurrency == "ZWG" ? tx.TaxCode : "";

                if (tx.TaxID == 1)
                {
                    scode += tcode + "0" + Math.Round(tx.SalesAmountWithTax * 100, 0).ToString();
                }
                else if (tx.TaxID == 2)
                {
                    scode += tcode + "0.000" + (tx.SalesAmountWithTax * 100).ToString();
                }
                else if (tx.TaxID == 3 || tx.TaxID == 4)
                {
                    scode += tcode + ((double)tx.TaxPercent).ToString("F2") + Math.Round(tx.TaxAmount * 100, 0).ToString() + Math.Round(tx.SalesAmountWithTax * 100, 0).ToString();
                }

                sHash += scode;
            }
            return sHash;
        }
    }
}
