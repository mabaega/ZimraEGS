# Manager Io - Zimra FDMS Integration

## ZimraEGS Application Testing Guide


## Purpose
This guide aims to assist users in integrating and testing the ZimraEGS application, from device registration to invoice reporting.

---

## What ZimraEGS Does
1. **Data Conversion**: ZimraEGS receives data from the relay, converts it to the required ZIMRA FDMS format, and sends it.
2. **Data Storage**: Responses from ZIMRA FDMS are stored in Manager.IO's business data. ZatcaEGS not recording any user data.

---

## Steps to Use ZimraEGS

### 1. Adding a New Device
#### Access ZIMRA Fiscalisation Data Management System
1. Open this URL: [ZIMRA FDMS Test Environment](http://fdmsops.zimra.co.zw/fdms-public/add-device).
2. Complete all fields in the **Add New Device** form:
   - **TIN** and **Taxpayer** should follow the required format but do not need to be real.
   - Select the following options:
     - **Device Model Name**: *Server*
     - **Integrator**: *Self (server to server)*
3. Save all the information provided after filling out the form.

---

### 2. Creating New Business Data in Manager.IO
1. Open **Manager.IO**.
2. Create a new business data entry.
3. Enable the **Sales Invoice** module.
4. Create a new invoice and add the relay URL below in the invoice settings:
   - `https://zimraegs.azurewebsite.net/relay`
5. Save, then click the **Relay** button on the **Invoice View** page. You will be redirected to the **Register Device** page.

---

### 3. Device Registration
#### A. Update Business Setup
1. On the **Register Device** page, click the **Update Business Setup** button.
2. If successful, its created custimfields needed for integration.
3. Proceed to the next step.

#### B. Fill Registration Form
1. Complete the form with the data obtained from the ZIMRA Fiscalisation Data Management System.
2. Click **Verify Taxpayer**. If verification is successful, its will show Tax Payer Information.
3. Proceed to the next step.

#### C. Register and Configure
1. Click the **Register Device** button.
   - If successful, we get Device Certificate from server.
   - Continue to the **GetConfig** step.
2. Click the **GetConfig** button.
   - If successful, Registration is Done, and Manager ready for comunication with Zimra Server.
   - Click the **Download** button. All Integration Information will be automatically saved in the business data.

The device is now ready for invoice reporting.

---

### 4. Testing Invoice Reporting
#### A. Set Base Currenzy
1. In Manager, goto Setting - Currencies - Base Currency
2. Set 'ZWG' on Code and Symbol, and Click Save or Update

#### B. Configure Default Form
1. Remove Previously Invoice
2. Create a new default form.
3. Checklist 'Custom Themes' and add 'ZimTheme'
4. Checklist 'Show TaxAmount Column' option
5. Checklist 'Relay' option Add the relay URL (`https://zimraegs.azurewebsite.net/relay`) to the default form.
6. Save the form settings.

#### C. Create a New Invoice
1. Create a new invoice with the following details:
   - Date and Reference are disabled, it will automaticaly add when Invoice Reporting.
   - Add Customers, you can use the default test customer
   - Add Item, make sure All Item Should Have HSCode and TaxCode.
   - **Payment Type** and **Payment Amount** (the payment amount must equal the total invoice amount).
   - Leave other fields blank for now and Click Create.
2. Click the **Relay** button on the **Invoice View** page. You will be redirected to the **Open Day** page.

#### D. Open a Fiscal Day
1. On the **Open Day** page, click the **Open Fiscal Day** button.
2. Check the results and ensure the **FiscalDay** status is successful (FiscalDayStatusOpened).
3. Back to Manager Invoice view page.

#### E. Submit the Invoice
1. Resend the invoice via the **Relay** button.
2. Ensure the invoice format matches ZIMRA FDMS specifications.
3. Click the **Submit Invoice** button.
   - If successful with no errors in server response, a QRCode will be displayed on the **Invoice View** page.
4. Make and submit some Invoice for testing.

---

### 5. Closing the Fiscal Day
#### Close the Fiscal Day:
1. Resend an invoice with the **APPROVED** status.
2. The **Close Day** button will appear.
3. Click the **Close Day** button.
   - If successful, the Fiscal Day status will update to **FiscalDayClosed**.

---

## Key Points to Understand
#### Data Handling:
1. **Invoice Date**: Adjusted to the date when the receipt is first submitted to the server.
2. **Invoice Number**: Automatically generated using the Fiscal Day number and Invoice Counter.
3. **Customer Data**: Should be Complete.
4. **Inventory Item**: Should have TaxCode and HSCode
5. **Payment Amount**: Should Equal with Invoice Total Amount.

#### Error Handling:
1. If submission fails, fix the data and resend it.
2. New invoices cannot be sent until failed invoices are resolved.
3. If corrections fail, manually close the Fiscal Day via the ZIMRA FDMS Test Environment website.
4. After manual closure, open a new Fiscal Day for new invoices.
5. Due to limited developer support, users must deeply understand the error-handling process.

---

#### Feedback:
....

Thanks.
