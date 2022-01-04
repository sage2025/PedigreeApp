using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.Entity.InMemoryOnly
{
    public class GrainItem
    {
        public int HorseId { get; set; }
        public double Inb { get; set; }
        public double Anc { get; set; }
        public double Ancs { get; set; }
        public double Anck { get; set; }

        private Dictionary<int, double> inbp = new Dictionary<int, double>();
        private Dictionary<int, double> ancp = new Dictionary<int, double>();
        private Dictionary<int, double> ancsp = new Dictionary<int, double>();
        private Dictionary<int, double> anckp = new Dictionary<int, double>();
        public Dictionary<int, double> Inbp { get { return inbp; } }
        public Dictionary<int, double> Ancp { get { return ancp; } }
        public Dictionary<int, double> Ancsp { get { return ancsp; } }
        public Dictionary<int, double> Anckp { get { return anckp; } }

        public void SetInbp(int aid, double val)
        {
            inbp[aid] = val;
        }
        public void SetAncp(int aid, double val)
        {
            ancp[aid] = val;
        }
        public void SetAncsp(int aid, double val)
        {
            ancsp[aid] = val;
        }
        public void SetAnckp(int aid, double val)
        {
            anckp[aid] = val;
        }
    }
}
