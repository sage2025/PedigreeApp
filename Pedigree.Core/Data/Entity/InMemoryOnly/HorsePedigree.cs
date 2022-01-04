using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pedigree.Core.Data.Entity.InMemoryOnly
{
    public class HorsePedigree
    {
        private int _startHorseId;
        private List<PedItem> _data;
        private Dictionary<int, HorseHeirarchy> _horses;

        public HorsePedigree(int startHorseId, List<HorseHeirarchy> hierarchy)
        {
            _startHorseId = startHorseId;
            _data = new List<PedItem>();
            _horses = new Dictionary<int, HorseHeirarchy>();

            foreach (var horse in hierarchy)
            {
                PedItem item = new PedItem(horse.Id);

                item.FatherId = horse.FatherId;
                item.MotherId = horse.MotherId;

                _data.Add(item);
                _horses[horse.Id] = horse;
            }

            Sort();

            MakeHorsesTree();
        }
        public int StartHorseId
        {
            get
            {
                return _startHorseId;
            }
            set
            {
                _startHorseId = value;
            }
        }

        private void Sort()
        {
            int i;
            int nl = _data.Count;
            int[] ord = new int[nl];
            int k;
            int[] ngen = new int[nl];
            int[] prc = Sire;
            int[] mrc = Dam;
            int nbit;
            int ip, im;

            for (i = 0; i < nl; i++) ord[i] = -1;

            k = 0;
            for (i = 0; i < nl; i++)
            {
                ngen[i] = -1;
                if (prc[i] == -1 && mrc[i] == -1)
                {
                    ngen[i] = 0;
                    ord[k] = i;
                    k++;
                }
            }

            nbit = 0;
            while (k < nl && nbit <= 50)
            {
                nbit++;
                for (i = 0; i < nl; i++)
                {
                    if (ngen[i] < 0)
                    {
                        ip = prc[i];
                        im = mrc[i];
                        bool t = true;
                        if (ip != -1 && ngen[ip] < 0) t = false;
                        if (im != -1 && ngen[im] < 0) t = false;
                        if (t)
                        {
                            int ii = ip == -1 ? 0 : ngen[ip];
                            if (im != -1) ii = Math.Max(ii, ngen[im]);
                            ngen[i] = ii + 1;
                            ord[k] = i;
                            k++;
                        }
                    }
                }
            }

            var tmp = new List<PedItem>();
            for(i = 0; i < nl; i++)
            {
                try
                {
                    tmp.Add(_data[ord[i]]);
                } 
                catch(Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
            }

            _data = tmp;
        }

        public List<HorseHeirarchy> Horses
        {
            get { return _horses.Values.ToList(); }
            set
            {
                foreach (var horse in value)
                {
                    _horses[horse.Id] = horse;
                }
            }
        }

        public int[] Ids
        {
            get
            {
                return _data.Select(x => x.HorseId).ToArray();
            }
        }

        public int[] Sire
        {
            get
            {
                return _data.Select(x => x.FatherId > 0 ? Array.IndexOf(Ids, x.FatherId) : -1).ToArray();
            }
        }
        public int[] Dam
        {
            get
            {
                return _data.Select(x => x.MotherId > 0 ? Array.IndexOf(Ids, x.MotherId) : -1).ToArray();
            }
        }

        private void MakeHorsesTree()
        {
            foreach (var horse in _horses.Values)
            {
                PedItem item = _data.FirstOrDefault(x => x.HorseId == horse.Id);

                if (item.FatherId > 0 && _horses.ContainsKey(item.FatherId))
                    horse.Father = _horses[item.FatherId];
                
                if (item.MotherId > 0 && _horses.ContainsKey(item.MotherId))                
                    horse.Mother = _horses[item.MotherId];                
            }
        }
        public int Count
        {
            get
            {
                return _data.Count;
            }
        }

        public Horse GetHorse(int horseId)
        {
            return _horses[horseId];
        }
        public Horse GetHorseByIndex(int i)
        {
            return _horses[Ids[i]];
        }

        public Horse GetStartHorse()
        {
            return _horses[_startHorseId];
        }

        public void ToPedigFile()
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter("D:\\Work\\Jobs\\PedigreeHorse\\tmp\\pedig");

            int[] sire = Sire;
            int[] dam = Dam;
            for (int i = 0; i < _data.Count; i++)
            {
                var horse = _horses[_data[i].HorseId];
                file.WriteLine("{0} {1} {2} {3} {4} {5} {6} {7}", i + 1, sire[i] + 1, dam[i] + 1, horse.Age, horse.Sex == "Male" ? 1 : 2, _data[i].HorseId, _data[i].FatherId, _data[i].MotherId);
            }

            file.Close();
        }

        public void ToAncestorFile(int[] ancestorIds)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter("D:\\Work\\Jobs\\PedigreeHorse\\tmp\\ancestor");

            foreach (var ancestorId in ancestorIds)
            {
                var index = Array.IndexOf(Ids, ancestorId);
                file.WriteLine("{0}", index > -1 ? index + 1 : 0);
            }

            file.Close();
        }
    }
}
