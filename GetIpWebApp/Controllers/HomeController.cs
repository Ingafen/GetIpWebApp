using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Microsoft.AspNetCore.Mvc;
using GetIpWebApp.Models;

namespace GetIpWebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        ViewData["IpAddress"] = GetLocalIpAddress();
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
    string GetLocalIpAddress()  
    {  
        UnicastIPAddressInformation? mostSuitableIp = null;  
        var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();  
        foreach (var network in networkInterfaces)  
        {  
            if (network.OperationalStatus != OperationalStatus.Up)  
                continue;  
      
            var properties = network.GetIPProperties();  
            if (properties.GatewayAddresses.Count == 0)  
                continue;  
      
            foreach (var address in properties.UnicastAddresses)  
            {  
                if (address.Address.AddressFamily != AddressFamily.InterNetwork)  
                    continue;  
      
                if (IPAddress.IsLoopback(address.Address))  
                    continue;  
      
                if (!address.IsDnsEligible)  
                {  
                    if (mostSuitableIp == null)  
                        mostSuitableIp = address;  
                    continue;  
                }  
      
                // The best IP is the IP got from DHCP server  
                if (address.PrefixOrigin != PrefixOrigin.Dhcp)  
                {  
                    if (mostSuitableIp is not { IsDnsEligible: true })  
                        mostSuitableIp = address;  
                    continue;  
                }  
                return address.Address.ToString();  
            }  
        }  
        return mostSuitableIp != null  
            ? mostSuitableIp.Address.ToString()  
            : "";  
    } 
}