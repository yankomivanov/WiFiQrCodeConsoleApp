using QRConsoleApp.Host;

try
{
    WifiQrConsoleApp.Run();
    Console.ReadKey();
}
catch (Exception e)
{
    Console.WriteLine("!!! Critical exception !!!");
    Console.WriteLine(e);
}
