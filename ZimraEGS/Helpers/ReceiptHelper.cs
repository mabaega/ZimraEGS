using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ZimraEGS.ApiClient.Enums;
using ZimraEGS.ApiClient.Helpers;
using ZimraEGS.ApiClient.Models;
using ZimraEGS.Models;

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

            receipt.ReceiptCurrency = string.IsNullOrEmpty(relayData.CurrencyCode) ? "ZWG" : relayData.CurrencyCode;
            receipt.ReceiptCounter = relayData.BusinessReference.LastReceiptCounter + 1;

            //Handle Rejected Invoice
            if (relayData.ApprovalStatus != "REJECTED") //New Invoice
            {
                //receipt.ReceiptGlobalNo = relayData.DeviceStatus.LastReceiptGlobalNo + 1;
                receipt.ReceiptGlobalNo = relayData.BusinessReference.LastReceiptGlobalNo + 1;
                receipt.ReceiptDate = DateTimeOffset.Now;
            }
            else
            {
                receipt.ReceiptGlobalNo = relayData.InvReceiptGlobalNo;
                receipt.ReceiptDate = relayData.ReceiptDate ?? DateTimeOffset.Now;
            }

            int InvNumber = relayData.DeviceStatus.LastFiscalDayNo ?? 1;
            receipt.InvoiceNo = $"{InvNumber:D4}-{receipt.ReceiptCounter:D4}";

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
                    ReceiptID = relayData.ReceiptID,
                    DeviceID = relayData.DeviceIDRef,
                    ReceiptGlobalNo = relayData.ReceiptGlobalNoRef,
                    FiscalDayNo = relayData.FiscalDayNoRef,
                    ReceiptRefDate = relayData.ReceiptRefDate 
                    
                };
            }

            receipt.ReceiptLinesTaxInclusive = relayData.ManagerInvoice?.AmountsIncludeTax ?? false;

            receipt.ReceiptLines = GetReceiptLine();

            List<ReceiptTax> listofreceipttax = GetNormalReceiptTaxs([.. receipt.ReceiptLines], receipt.ReceiptLinesTaxInclusive);

            receipt.ReceiptTaxes = listofreceipttax.GroupBy(x => new { x.TaxCode, x.TaxID, x.TaxPercent })
                                  .Select(g => new ReceiptTax
                                  {
                                      TaxCode = g.Key.TaxCode,
                                      TaxID = g.Key.TaxID,
                                      TaxPercent = g.Key.TaxPercent,
                                      TaxAmount = g.Sum(x => x.TaxAmount),
                                      SalesAmountWithTax = g.Sum(x => x.SalesAmountWithTax)
                                  }).OrderBy(o => o.TaxID).ToList();

            receipt.ReceiptPayments = GetPayments();

            receipt.ReceiptTotal = receipt.ReceiptTaxes.Sum(x => x.SalesAmountWithTax);

            receipt.ReceiptPrintForm = relayData.ReceiptPrintForm == "Receipt48" ? ReceiptPrintForm.Receipt48 : ReceiptPrintForm.InvoiceA4;

            //string ReceiptTaxsHash = GetReceiptTaxsHash([.. receipt.ReceiptTaxes], receipt.ReceiptCurrency);
            string ReceiptTaxsHash = GetReceiptTaxsHash([.. listofreceipttax], receipt.ReceiptCurrency);

            string SourcesHash =
                ((relayData.CertificateInfo.DeviceID.ToString() +
                receipt.ReceiptType.ToString().ToUpper() +
                receipt.ReceiptCurrency +
                receipt.ReceiptGlobalNo.ToString() +
                receipt.ReceiptDate.ToString("yyyy-MM-ddTHH:mm:ss") +
                (receipt.ReceiptTotal * 100).ToString("F0") +
                ReceiptTaxsHash));

            if (receipt.ReceiptCounter > 1)
            {
                SourcesHash += relayData.BusinessReference.LastReceiptHash;
            }

            Console.WriteLine("Sources Hash String: " + SourcesHash);

            // Generate hash
            byte[] hashByte = RSA_CryptoHelper.ComputeSHA256Hash(SourcesHash);
            Console.WriteLine("Hash (Base64): " + Convert.ToBase64String(hashByte));

            // Generate signature
            //ECDsa privateKey = relayData.CertificateInfo.GetPrivateKey(); 
            RSA privateKey = RSA_CryptoHelper.ConvertPrivateKeyFromBase64(relayData.CertificateInfo.PrivateKey);
            byte[] signatureByte = RSA_CryptoHelper.SignDocument(privateKey, hashByte);
            Console.WriteLine("Signature (Base64): " + Convert.ToBase64String(signatureByte));

            // Set signature data
            receipt.ReceiptDeviceSignature = new SignatureData()
            {
                Hash = hashByte,
                Signature = signatureByte ?? Encoding.UTF8.GetBytes("")
            };

            // Verify signature
            X509Certificate2 deviceCertificate = RSA_CryptoHelper.ConvertCertificateFromBase64(relayData.CertificateInfo.DeviceCertificate);
            bool isValid = RSA_CryptoHelper.VerifySignature(deviceCertificate, hashByte, signatureByte);

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

                rl.ReceiptLineType = ZimraEGS.ApiClient.Enums.ReceiptLineType.Sale;
                rl.ReceiptLineNo = rw;
                rl.ReceiptLineName = GetFirstNonNullOrEmpty(l.LineDescription, l.Item.Name, l.Item.ItemName);
                rl.ReceiptLineHSCode = HsCode.PadLeft(8, '0');
                rl.ReceiptLineQuantity = Math.Round(l.Qty, 2);
                rl.ReceiptLinePrice = Math.Round(unitPrice, 4);

                if (relayData.ReceiptType == ReceiptType.CreditNote)
                {
                    rl.ReceiptLinePrice = -1 * rl.ReceiptLinePrice;
                }

                rl.ReceiptLineTotal = Math.Round(l.Qty * (double)rl.ReceiptLinePrice, 2);

                if (l.TaxCode != null && l.TaxCode.Name.Contains('—'))
                {
                    var taxcode = l.TaxCode.Name.Split("—");
                    //rl.TaxCode = taxcode[2].Trim();
                    rl.TaxID = GetIntValue(taxcode[1].Trim());
                    rl.TaxPercent = rl.TaxID == 1 ? null : taxRate;
                }
                else
                {
                    //rl.TaxCode = "A";
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

                    rl.ReceiptLineType = ZimraEGS.ApiClient.Enums.ReceiptLineType.Discount;
                    rl.ReceiptLineNo = rw;
                    rl.ReceiptLineName = "Discount: " + GetFirstNonNullOrEmpty(l.LineDescription, l.Item.Name, l.Item.ItemName);
                    rl.ReceiptLineHSCode = HsCode;
                    rl.ReceiptLineQuantity = Math.Round(l.Qty, 2);
                    rl.ReceiptLinePrice = Math.Round(-1 * (unitPrice), 4);

                    if (relayData.ReceiptType == ReceiptType.CreditNote)
                    {
                        rl.ReceiptLinePrice = -1 * rl.ReceiptLinePrice;
                    }

                    rl.ReceiptLineTotal = Math.Round(l.Qty * (double)rl.ReceiptLinePrice, 2);

                    if (l.TaxCode != null && l.TaxCode.Name.Contains('—'))
                    {
                        var taxcode = l.TaxCode.Name.Split('—');
                        //rl.TaxCode = taxcode[2].Trim();
                        rl.TaxID = GetIntValue(taxcode[1].Trim());
                        rl.TaxPercent = rl.TaxID == 1 ? null : taxRate;
                    }
                    else
                    {
                        //rl.TaxCode = "A";
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

        private List<ReceiptTax> GetNormalReceiptTaxs(List<ReceiptLine> receiptLines, bool taxInclusive)
        {
            List<ReceiptTax> ls = new List<ReceiptTax>();

            foreach (ReceiptLine ln in receiptLines)
            {
                ReceiptTax receiptTax = new ReceiptTax();
                //receiptTax.TaxCode = ln.TaxCode;
                receiptTax.TaxID = ln.TaxID;
                if (ln.TaxPercent != null) { receiptTax.TaxPercent = ln.TaxPercent; }

                var tx = (double)(ln.TaxPercent == null ? 0 : ln.TaxPercent);
                double tpersen = tx / 100;


                if (taxInclusive)
                {
                    double excTaxAmount = ln.ReceiptLineTotal / (1 + tpersen);
                    receiptTax.TaxAmount = Math.Round(ln.ReceiptLineTotal - excTaxAmount, 2);
                    receiptTax.SalesAmountWithTax = ln.ReceiptLineTotal;
                }
                else
                {
                    receiptTax.TaxAmount = Math.Round(ln.ReceiptLineTotal * tpersen, 2);
                    receiptTax.SalesAmountWithTax = ln.ReceiptLineTotal + receiptTax.TaxAmount;
                }

                receiptTax.LineNumber = ln.ReceiptLineNo;

                ls.Add(receiptTax);
            }

            return ls.OrderBy(x => x.TaxID).OrderBy(x => x.LineNumber).ToList();
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
                        PaymentAmount = ((relayData.ReceiptType == ReceiptType.CreditNote) ? -1 : 1) * relayData.PaymentAmount1
                   
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
                        PaymentAmount = ((relayData.ReceiptType == ReceiptType.CreditNote) ? -1 : 1) * relayData.PaymentAmount2
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

        //private string GetReceiptTaxsHash(List<ReceiptTax> receiptTaxs, string receiptCurrency)
        //{
        //    string sHash = string.Empty;

        //    foreach (ReceiptTax tx in receiptTaxs)
        //    {
        //        string scode = "";
        //        string tcode = receiptCurrency == "ZWG" ? tx.TaxCode : "";

        //        if (tx.TaxID == 1)
        //        {
        //            scode += tcode + "0" + Math.Round(tx.SalesAmountWithTax * 100, 0).ToString();
        //        }
        //        else if (tx.TaxID == 2)
        //        {
        //            scode += tcode + "0.000" + (tx.SalesAmountWithTax * 100).ToString();
        //        }
        //        else if (tx.TaxID == 3 || tx.TaxID == 4)
        //        {
        //            scode += tcode + ((double)tx.TaxPercent).ToString("F2") + Math.Round(tx.TaxAmount * 100, 0).ToString() + Math.Round(tx.SalesAmountWithTax * 100, 0).ToString();
        //        }

        //        sHash += scode;
        //    }
        //    return sHash;
        //}

    }
}
