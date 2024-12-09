using System.Text;
using Zimra.ApiClient;
using Zimra.ApiClient.Enums;
using Zimra.ApiClient.Models;
using ZimraEGS.Models;

namespace ZimraEGS.Helpers
{
    public static class FiscalDayProcessor
    {
        public static FiscalDaySummary CombineFiscalDaySummaries(FiscalDaySummary previousSummary, FiscalDaySummary newSummary)
        {
            // Combine the lists from the previous and new summaries
            var combinedReceipts = previousSummary.TaxSummaries
                .Concat(newSummary.TaxSummaries)
                .GroupBy(r => new { r.FiscalDayNo, r.ReceiptType, r.ReceiptCurrency, r.TaxCode, r.TaxPersen })
                .Select(group => new TaxSummary
                {
                    FiscalDayNo = group.Key.FiscalDayNo,
                    ReceiptType = group.Key.ReceiptType,
                    ReceiptCurrency = group.Key.ReceiptCurrency,
                    TaxCode = group.Key.TaxCode,
                    TaxPersen = group.Key.TaxPersen,
                    TaxAmount = group.Sum(r => r.TaxAmount),
                    SalesAmountWithTax = group.Sum(r => r.SalesAmountWithTax)
                })
                .ToList();

            var combinedPayments = previousSummary.PaymentSummaries
                .Concat(newSummary.PaymentSummaries)
                .GroupBy(p => new { p.FiscalDayNo, p.ReceiptCurrency, p.MoneyTypeCode })
                .Select(group => new PaymentSummary
                {
                    FiscalDayNo = group.Key.FiscalDayNo,
                    ReceiptCurrency = group.Key.ReceiptCurrency,
                    MoneyTypeCode = group.Key.MoneyTypeCode,
                    PaymentAmount = group.Sum(p => p.PaymentAmount)
                })
                .ToList();

            // Return the combined and grouped summary
            return new FiscalDaySummary
            {
                TaxSummaries = combinedReceipts,
                PaymentSummaries = combinedPayments
            };
        }

        // Example function to generate FiscalDaySummary from receipt data
        public static FiscalDaySummary GenerateFiscalDaySummary(Receipt receiptData, int fiscalDayNumber)
        {
            var fiscalDayNo = fiscalDayNumber;
            var receiptType = receiptData.ReceiptType;
            var receiptCurrency = receiptData.ReceiptCurrency;

            // Generate Tax Summary
            var taxSummary = receiptData.ReceiptTaxes
                .GroupBy(t => new { t.TaxCode, t.TaxPersenString })
                .Select(group => new TaxSummary
                {
                    FiscalDayNo = fiscalDayNo,
                    ReceiptType = receiptType,
                    ReceiptCurrency = receiptCurrency,
                    TaxCode = group.Key.TaxCode,
                    TaxPersen = group.Key.TaxPersenString,
                    TaxAmount = group.Sum(g => g.TaxAmount),
                    SalesAmountWithTax = group.Sum(g => g.SalesAmountWithTax)
                })
                .ToList();

            // Generate Payment Summary
            var paymentSummary = receiptData.ReceiptPayments
                .GroupBy(p => p.MoneyTypeCode)
                .Select(group => new PaymentSummary
                {
                    FiscalDayNo = fiscalDayNo,
                    ReceiptCurrency = receiptCurrency,
                    MoneyTypeCode = group.Key,
                    PaymentAmount = group.Sum(p => p.PaymentAmount)
                })
                .ToList();

            // Return FiscalDaySummary for the current receipt data
            return new FiscalDaySummary
            {
                TaxSummaries = taxSummary,
                PaymentSummaries = paymentSummary
            };
        }
        public static double? ToNullableDouble(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            return double.TryParse(input, out double result) ? (double?)result : null;
        }
        public static List<FiscalDayCounter> GetFiscalDayCounter(FiscalDaySummary fiscalDaySummary)
        {
            List<FiscalDayCounter> ls = new List<FiscalDayCounter>();

            foreach (var item in fiscalDaySummary.TaxSummaries)
            {
                switch (item.ReceiptType)
                {
                    case ReceiptType.FiscalInvoice:
                        ls.Add(new FiscalDayCounter
                        {
                            FiscalCounterType = FiscalCounterType.SaleByTax,
                            FiscalCounterCurrency = item.ReceiptCurrency,
                            FiscalCounterTaxPercent = item.TaxPersen?.ToNullableDouble(),
                            FiscalCounterValue = item.SalesAmountWithTax
                        });
                        ls.Add(new FiscalDayCounter
                        {
                            FiscalCounterType = FiscalCounterType.SaleTaxByTax,
                            FiscalCounterCurrency = item.ReceiptCurrency,
                            FiscalCounterTaxPercent = item.TaxPersen?.ToNullableDouble(),
                            FiscalCounterValue = item.TaxAmount
                        });
                        break;

                    case ReceiptType.CreditNote:
                        ls.Add(new FiscalDayCounter
                        {
                            FiscalCounterType = FiscalCounterType.CreditNoteByTax,
                            FiscalCounterCurrency = item.ReceiptCurrency,
                            FiscalCounterTaxPercent = item.TaxPersen?.ToNullableDouble(),
                            FiscalCounterValue = item.SalesAmountWithTax
                        });
                        ls.Add(new FiscalDayCounter
                        {
                            FiscalCounterType = FiscalCounterType.CreditNoteTaxByTax,
                            FiscalCounterCurrency = item.ReceiptCurrency,
                            FiscalCounterTaxPercent = item.TaxPersen?.ToNullableDouble(),
                            FiscalCounterValue = item.TaxAmount
                        });
                        break;

                    case ReceiptType.DebitNote:
                        ls.Add(new FiscalDayCounter
                        {
                            FiscalCounterType = FiscalCounterType.DebitNoteByTax,
                            FiscalCounterCurrency = item.ReceiptCurrency,
                            FiscalCounterTaxPercent = item.TaxPersen?.ToNullableDouble(),
                            FiscalCounterValue = item.SalesAmountWithTax
                        });
                        ls.Add(new FiscalDayCounter
                        {
                            FiscalCounterType = FiscalCounterType.DebitNoteTaxByTax,
                            FiscalCounterCurrency = item.ReceiptCurrency,
                            FiscalCounterTaxPercent = item.TaxPersen?.ToNullableDouble(),
                            FiscalCounterValue = item.TaxAmount
                        });
                        break;

                    default:
                        throw new InvalidOperationException($"Unsupported ReceiptType: {item.ReceiptType}");
                }
            }

            foreach (var item in fiscalDaySummary.PaymentSummaries)
            {
                ls.Add(new FiscalDayCounter
                {
                    FiscalCounterType = FiscalCounterType.BalanceByMoneyType,
                    FiscalCounterCurrency = item.ReceiptCurrency,
                    FiscalCounterMoneyType = item.MoneyTypeCode,
                    FiscalCounterValue = item.PaymentAmount
                });
            }

            return ls;
        }
        public static CloseDayRequest GenerateCloseDayRequest(RelayData model)
        {
            var closeDayRequest = new CloseDayRequest();
            try
            {
                closeDayRequest.FiscalDayNo = model.DeviceStatus.LastFiscalDayNo ?? 0;

                closeDayRequest.FiscalDayCounters = GetFiscalDayCounter(model.FiscalDaySummary);

                var counterHash = ToHashString(closeDayRequest.FiscalDayCounters.ToList());

                string SourcesHash = model.CertificateInfo.DeviceID.ToString() +
                    closeDayRequest.FiscalDayNo +
                    model.DeviceStatus.FiscalDayClosed?.ToString("yyyy-MM-dd") +
                    counterHash;

                byte[] hashByte = DigitalSignatureUtility.ComputeHash(SourcesHash.ToUpper());
                byte[] signatureByte = DigitalSignatureUtility.GenerateSignature(hashByte, model.CertificateInfo.PrivateKey);

                closeDayRequest.FiscalDayDeviceSignature = new SignatureData()
                {
                    Hash = hashByte,
                    Signature = signatureByte ?? Encoding.UTF8.GetBytes("")
                };

                closeDayRequest.ReceiptCounter = model.ReceiptCounterRef;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return closeDayRequest;

        }

        public static string ToHashString(this List<FiscalDayCounter> fiscalDayCounters)
        {
            var stringHash = string.Empty;
            foreach (var item in fiscalDayCounters)
            {
                if (item.FiscalCounterType < FiscalCounterType.BalanceByMoneyType)
                {
                    stringHash += item.FiscalCounterType.ToString();
                    stringHash += item.FiscalCounterCurrency;
                    stringHash += item.FiscalCounterTaxPercent == null ? string.Empty : ((double)item.FiscalCounterTaxPercent).ToString("N2");
                    stringHash += item.FiscalCounterValue * 100;
                }
                else if (item.FiscalCounterType == FiscalCounterType.BalanceByMoneyType)
                {
                    stringHash += item.FiscalCounterType.ToString();
                    stringHash += item.FiscalCounterCurrency;
                    stringHash += item.FiscalCounterMoneyType;
                    stringHash += item.FiscalCounterValue * 100;
                }
            }

            return stringHash.ToUpper();
        }
    }
}
