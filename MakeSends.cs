namespace ViberSender2017
{
    internal class MakeSends
    {
        public bool SendSms(string number, string text, bool first, string path = null)
        {
            if (first && !WinApi.StartWork())
                return false;
            WinApi.ClickNumber();
            WinApi.EnterNumber(number);
            WinApi.ClickMessage();
            if (path != null)
                return WinApi.SendMsg(text, path, true);
            return WinApi.SendMsg(text, (string)null, false);
        }
    }
}
