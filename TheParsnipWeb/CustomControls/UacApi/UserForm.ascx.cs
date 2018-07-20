using System;
using UacApi;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TheParsnipWeb
{
    public partial class UserForm1 : System.Web.UI.UserControl
    {
        Account MyAccount = new Account();

        protected void Page_Load(object sender, EventArgs e)
        {
            /*if (e != null)
            {
                MyAccount.LogIn(e.ToString());
            }*/
        }

        void UpdateForm()
        {
            username.Text = MyAccount.Username;
            email.Text = MyAccount.Email;
            //updatePwd()
            forename.Text = MyAccount.Forename;
            surname.Text = MyAccount.Surname;
            //updateDob()
            address1.Text = MyAccount.Address1;
            address2.Text = MyAccount.Address2;
            address3.Text = MyAccount.Address3;
            postCode.Text = MyAccount.Postcode;
            mobilePhone.Text = MyAccount.MobilePhone;
            homePhone.Text = MyAccount.HomePhone;
            workPhone.Text = MyAccount.WorkPhone;
            // updateDateTimeCreated()
            accountType.Value = MyAccount.AccountType;
            accountStatus.Value = MyAccount.AccountStatus;
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            if (Validate())
            {
                WriteToObj();
                MyAccount.DbInsert(password1.Text);
            }

            bool Validate()
            {
                if(validateUsername() &&
                validateEmail() &&
                validatePwd() &&
                validateForename() &&
                validateSurname() &&
                validateAddress1() &&
                validateAddress2() &&
                validateAddress3() &&
                validatePostCode() &&
                validateMobilePhone() &&
                validateHomePhone() &&
                validateWorkPhone() &&
                validateDateTimeCreated() &&
                validateAccountType() &&
                validateAccountStatus())
                {
                    SuccessText.Text = "Account created successfully!";
                    Success.Style.Add("display", "true");

                    return true;
                }
                else
                {
                    WarningText.Text = "Failed to create account";
                    Warning.Style.Add("display", "true");
                    return false;
                }
                
                
                bool validateUsername()
                {
                    if(username.Text.Trim().Length == 0)
                    {
                        username.CssClass = "invalid";
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

                bool validateEmail()
                {
                    string EmailAddress = email.Text.Trim();

                    if (EmailAddress.Length != 0)
                    {
                        int AsperandIndex; //Index of "@" sign
                        int PointIndex; //Index of "."
                        string Username;
                        string MailServer;
                        string DomainExtension;
                        
                        if (EmailAddress.Contains("@"))
                        {
                            int NoOfAsperand = EmailAddress.Split('@').Length - 1;
                            if (NoOfAsperand == 1)
                            {
                                AsperandIndex = EmailAddress.IndexOf("@");
                                Username = EmailAddress.Substring(0, AsperandIndex);
                                //MyLog.Debug("Email: Username = " + Username);
                                if (EmailAddress.Substring(AsperandIndex + 1, EmailAddress.Length - AsperandIndex - 1).Contains("."))
                                {
                                    PointIndex = EmailAddress.LastIndexOf('.');
                                    MailServer = EmailAddress.Substring(AsperandIndex + 1, PointIndex - AsperandIndex - 1);
                                    //MyLog.Debug("Email: Mail server = " + MailServer);
                                    DomainExtension = EmailAddress.Substring(PointIndex + 1, EmailAddress.Length - PointIndex - 1);
                                    return true;
                                }
                                else
                                {
                                    //MyLog.Warning("Email address domain does not contain a \".\". Email address will be blank!");
                                    email.CssClass = "invalid";
                                    return false;
                                }
                            }
                            else
                            {
                                //MyLog.Warning("Email address contains too many @'s. Email address will be blank!");
                                email.CssClass = "invalid";
                                return false;
                            }
                        }
                        else
                        {
                            //MyLog.Warning("Email address does not contain an \"@\" sign. Email address will be blank!");
                            email.CssClass = "invalid";
                            return false;
                        }
                    }
                    else
                    {
                        //Don't really need to be warned about blank fields.
                        //MyLog.Warning(String.Format("Email \"{0}\" is made up from blank characters! Email address will be blank!", EmailVal));
                        email.CssClass = "invalid";
                        return false;
                    }
                }

                bool validatePwd()
                {
                    if (password1.Text.Trim().Length == 0)
                    {
                        password1.CssClass = "invalid";
                        password2.CssClass = "invalid";
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

                bool validateForename(){
                    return true;
                }

                bool validateSurname()
                {
                    return true;
                }

                /*
                bool validateDob()
                {

                }
                */

                bool validateAddress1()
                {
                    return true;
                }

                bool validateAddress2()
                {
                    return true;
                }

                bool validateAddress3()
                {
                    return true;
                }

                bool validatePostCode()
                {
                    return true;
                }

                bool validateMobilePhone()
                {
                    return true;
                }

                bool validateHomePhone()
                {
                    return true;
                }

                bool validateWorkPhone()
                {
                    return true;
                }

                bool validateDateTimeCreated()
                {
                    return true;
                }

                bool validateAccountType()
                {
                    return true;
                }

                bool validateAccountStatus()
                {
                    return true;
                }
            }


            void WriteToObj()
            {
                MyAccount.Username = username.Text;
                MyAccount.Email = email.Text;
                //MyAccount.Pwd = password1.Text;
                MyAccount.Forename = forename.Text;
                MyAccount.Surname = surname.Text;
                //MyAccount.Dob = dob.Text;
                MyAccount.Address1 = address1.Text;
                MyAccount.Address2 = address2.Text;
                MyAccount.Address3 = address3.Text;
                MyAccount.Postcode = postCode.Text;
                MyAccount.MobilePhone = mobilePhone.Text;
                MyAccount.HomePhone = homePhone.Text;
                MyAccount.WorkPhone = workPhone.Text;
                MyAccount.DateTimeCreated = DateTime.Now;
                MyAccount.AccountType = accountType.Value;
                MyAccount.AccountStatus = accountStatus.Value;
                MyAccount.AccountType = accountType.Value;
                
            }
        }
    }
}