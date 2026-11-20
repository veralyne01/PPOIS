using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    public class CustomerWallet
    {
        public int sum { get; private set; }
        public Dictionary<Nominals, int> nominals { get; private set; } = new Dictionary<Nominals, int>();
    }
}
