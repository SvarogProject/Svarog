using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Tools {
    public static int UniqueRandomInt(ICollection<int> l, int min, int max) {
        var retVal = Random.Range(min, max);

        while (l.Contains(retVal)) {
            retVal = Random.Range(min, max);
        }

        return retVal;
    }

    public static string LocalIpAddress() {
        var localIp = "0.0.0.0";
        var host = Dns.GetHostEntry(Dns.GetHostName());

        foreach (var ip in host.AddressList) {
            if (ip.AddressFamily != AddressFamily.InterNetwork) continue;

            localIp = ip.ToString();

            break;
        }

        return localIp;
    }
}