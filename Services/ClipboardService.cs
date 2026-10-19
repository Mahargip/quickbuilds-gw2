using System;
using System.Threading;
using System.Windows.Forms;

namespace QuickBuilds.Services
{
    public static class ClipboardService
    {
        public static bool SetText(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;

            try
            {
                var thread = new Thread(() =>
                {
                    try
                    {
                        Clipboard.SetText(text);
                    }
                    catch
                    {
                        // Fallback retry once if clipboard was momentarily locked
                        Thread.Sleep(50);
                        try
                        {
                            Clipboard.SetText(text);
                        }
                        catch
                        {
                            // Ignored
                        }
                    }
                });

                thread.SetApartmentState(ApartmentState.STA);
                thread.IsBackground = true;
                thread.Start();
                thread.Join(500);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
