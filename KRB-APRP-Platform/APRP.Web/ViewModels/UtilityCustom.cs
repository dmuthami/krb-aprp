namespace APRP.Web.ViewModels
{
    public class UtilityCustom
    {
        public string FormatPhoneNumber(string phone)
        {
            //string str =  phone.Substring(1);
            string str = "+254" + phone.Substring(1);
            return str;
        }
    }
}
