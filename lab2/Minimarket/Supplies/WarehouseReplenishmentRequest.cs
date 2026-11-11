using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    class WarehouseReplenishmentRequest
    {
        Manufacturer manufacturer;
        Item item;
        int requestedAmount;
        bool isApproved;
    }
}
