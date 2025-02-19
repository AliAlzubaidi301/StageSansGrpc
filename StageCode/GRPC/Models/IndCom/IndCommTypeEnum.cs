using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.IndCom
{
    public enum IndCommTypeEnum
    {
        Undef = 0,
        Modbus_RS232_Slave = 1,
        Modbus_RS232_Master = 2,
        Modbus_TCPIP_Slave = 3,
        Modbus_TCPIP_Master = 4,
        Hilscher_CARD_Profibus = 5
    }
}
