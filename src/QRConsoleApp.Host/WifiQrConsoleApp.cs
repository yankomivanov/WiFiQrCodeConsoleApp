using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using static QRCoder.PayloadGenerator;

namespace QRConsoleApp.Host;

internal static class WifiQrConsoleApp
{
    public static void Run()
    {
        bool runApp = true;
        while (runApp)
        {
            WiFi? wifi = null;
            string ssid = string.Empty;

            while (wifi == null)
            {
                wifi = GetWifiFromConsole(out ssid);
            }

            Bitmap qrCodeAsBitmap = GetQrBitmap(wifi);
            GenerateAndSaveImage(qrCodeAsBitmap, ssid);
            runApp = CheckToRunAgain();
            Console.WriteLine();
        }
    }

    private static bool CheckToRunAgain()
    {
        Console.WriteLine();
        Console.WriteLine("Press 'y' to run again. Press any other key to exit the application...");
        return string.Equals(Console.ReadKey().KeyChar.ToString(), "y", StringComparison.InvariantCultureIgnoreCase);
    }

    private static void GenerateAndSaveImage(Bitmap qrCodeAsBitmap, string ssid)
    {
        string datePart = DateTime.Now.ToString("yyyy_MM_dd_hhmmss");
        var fileName = $"{ssid}_{datePart}.png";
        var dirName = $"results_{datePart}";

        Directory.CreateDirectory(dirName);

        qrCodeAsBitmap.Save(Path.Combine(dirName, fileName), ImageFormat.Png);
        Console.WriteLine($"{fileName} successfully created.");
    }

    private static Bitmap GetQrBitmap(WiFi wifi)
    {
        Console.WriteLine("Generating QR Code now..");
        string payload = wifi.ToString();

        QRCodeGenerator qrGenerator = new();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
        QRCode qrCode = new(qrCodeData);
        return qrCode.GetGraphic(20);
    }

    private static WiFi? GetWifiFromConsole(out string wifiSsid)
    {
        Console.WriteLine("Enter the WiFi's SSID (name) and press ENTER:");
        wifiSsid = Console.ReadLine()?.Trim() ?? string.Empty;
        Console.WriteLine($"SSID: {wifiSsid}");

        Console.WriteLine("Press 'y' to hide the password; press any other key to show the password...");
        var hidePassKey = Console.ReadKey().KeyChar.ToString();
        bool shouldHidePass = string.Equals(hidePassKey, "y", StringComparison.InvariantCultureIgnoreCase);
        Console.WriteLine();

        Console.WriteLine("Enter the WiFi's Password and press ENTER:");
        var password = shouldHidePass ? GetPassword() : Console.ReadLine();
        Console.WriteLine($"Password's length: {password?.Length ?? 0}");

        if (string.IsNullOrWhiteSpace(wifiSsid) || string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Ivalid WIFI SSID and/or Password. Try Again!");
            return null;
        }

        return new WiFi(wifiSsid, password, WiFi.Authentication.WPA);
    }
    private static string GetPassword()
    {
        var pass = string.Empty;
        ConsoleKey key;
        do
        {
            var keyInfo = Console.ReadKey(intercept: true);
            key = keyInfo.Key;

            if (key == ConsoleKey.Backspace && pass.Length > 0)
            {
                Console.Write("\b \b");
                pass = pass[0..^1];
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                Console.Write("*");
                pass += keyInfo.KeyChar;
            }
        } while (key != ConsoleKey.Enter);
        Console.WriteLine();
        return pass;
    }
}
