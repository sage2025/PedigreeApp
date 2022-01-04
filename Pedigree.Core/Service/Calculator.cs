using Pedigree.Core.Data.Entity;
using Pedigree.Core.Data.Entity.InMemoryOnly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pedigree.Core.Service
{
    public class Calculator
    {
        public List<Horse> CommonAncestors(List<HorseHeirarchy> horses1, List<HorseHeirarchy> horses2)
        {
            return horses1.Intersect(horses2, new CommonHorsesComparer()).ToList();
        }

        public double CalculateNCG(Horse horse, int maxgen)
        {
            int total = 0;
            for (int i = 1; i <= maxgen; i++)
            {
                total += (int)Math.Pow(2, i);
            }

            int cnt = CalculateNCG(horse, 0, maxgen);

            return total > 0 ? 100 * cnt / total : 0;
        }

        public int CalculateNCG(Horse horse, int gen, int maxgen)
        {
            int cnt = 0;

            if (gen > maxgen - 1) return 0;

            if (horse.Father != null)
            {
                cnt++;
                cnt += CalculateNCG(horse.Father, gen + 1, maxgen);
            }

            if (horse.Mother != null)
            {
                cnt++;
                cnt += CalculateNCG(horse.Mother, gen + 1, maxgen);
            }

            return cnt;
        }

        public double CalculateCGE(Horse horse, int gen, int maxgen = 10)
        {
            double cge = 0;

            if (gen > maxgen - 1) return 0;

            if (horse.Father != null)
            {
                cge += 1 / Math.Pow(2, gen + 1);
                cge += CalculateCGE(horse.Father, gen + 1, maxgen);
            }

            if (horse.Mother != null)
            {
                cge += 1 / Math.Pow(2, gen + 1);
                cge += CalculateCGE(horse.Mother, gen + 1, maxgen);
            }

            return cge;
        }

        /// <summary>
        /// Get inbreeding status of selected horse as like this format(5S x 6D)
        /// 5S: 5 generation on Sire side
        /// 6D: 6 generation on Dam side
        /// </summary>
        /// <param name="horse"></param>
        /// <returns>
        /// Key: HorseID, Value: Inbreeding status list
        /// </returns>
        public Dictionary<int, List<string>> Inbreeding(Horse horse, int maxGen = 10)
        {
            Dictionary<int, List<string>> inbreeding = new Dictionary<int, List<string>>();

            if (horse.Father != null) Inbreeding(horse.Father, 1, inbreeding, "S", maxGen);
            if (horse.Mother != null) Inbreeding(horse.Mother, 1, inbreeding, "D", maxGen);

            return inbreeding;
        }

        private void Inbreeding(Horse horse, int gen, Dictionary<int, List<string>> inbreeding, string sd, int maxGen)
        {
            if (gen > maxGen) return;

            if (!inbreeding.ContainsKey(horse.Id)) inbreeding[horse.Id] = new List<string>();
            inbreeding[horse.Id].Add($"{gen}{sd}");


            if (horse.Father != null)
            {
                Inbreeding(horse.Father, gen + 1, inbreeding, sd, maxGen);
            }

            if (horse.Mother != null)
            {
                Inbreeding(horse.Mother, gen + 1, inbreeding, sd, maxGen);
            }
        }

        public Dictionary<int, List<string>> Inbreeding(List<HorseHeirarchy> hierarchy, int gen=10)
        {
            Dictionary<int, List<string>> inbreeding = new Dictionary<int, List<string>>();

            foreach(var horse in hierarchy)
            {
                if (horse.Depth <= gen)
                {
                    if (!inbreeding.ContainsKey(horse.Id)) inbreeding[horse.Id] = new List<string>();
                    inbreeding[horse.Id].Add($"{horse.Depth}{horse.SD}");
                }
            }

            return inbreeding.Where(inbreed => inbreed.Value.Count > 1).ToDictionary(i => i.Key, i => i.Value);
        }
        public void GetAncestors(Horse horse, int gen, int maxgen, List<int> ancestors, Dictionary<int, int> dictGen = null)
        {
            if (gen > maxgen) return;

            if (horse.Father != null)
            {
                ancestors.Add(horse.Father.Id);
                if (dictGen != null) dictGen[horse.Father.Id] = gen;
                GetAncestors(horse.Father, gen + 1, maxgen, ancestors, dictGen);
            }
            if (horse.Mother != null)
            {
                ancestors.Add(horse.Mother.Id);
                if (dictGen != null) dictGen[horse.Mother.Id] = gen;
                GetAncestors(horse.Mother, gen + 1, maxgen, ancestors, dictGen);
            }
        }

        public double CalculateAGR(Horse horse1, Horse horse2)
        {
            Dictionary<int, int> dictGen1 = new Dictionary<int, int>();
            Dictionary<int, int> dictGen2 = new Dictionary<int, int>();
            List<int> commonAncestors = GetCommonAncestorsInGen(horse1, horse2, 10, dictGen1, dictGen2);

            double sum = 0;
            for (int i = 0; i < commonAncestors.Count; i++)
            {
                int id = commonAncestors[i];
                sum += Math.Pow(0.5, dictGen1[id] + dictGen2[id]);
            }
            return sum;
        }

        public List<int> GetCommonAncestorsInGen(Horse horse1, Horse horse2, int gen, Dictionary<int, int> dictGen1 = null, Dictionary<int, int> dictGen2 = null)
        {
            List<int> ancestors1 = new List<int>();
            GetAncestors(horse1, 1, gen, ancestors1, dictGen1);

            List<int> ancestors2 = new List<int>();
            GetAncestors(horse2, 1, gen, ancestors2, dictGen2);

            List<int> ancestors = ancestors1.Intersect(ancestors2, new CommonAncestorsIdComparer()).ToList();

            return ancestors;
        }

        public double CalculateGI(Horse horse)
        {
            double result = 0.0;
            if (horse.Father != null && horse.Father.Age > 0) result += horse.Age - horse.Father.Age;
            if (horse.Mother != null && horse.Mother.Age > 0) result += horse.Age - horse.Mother.Age;
            result /= 2;

            return result;
        }
        public double CalculateGDGS(Horse horse)
        {
            var father = horse.Father;
            if (father != null)
            {
                if (father.Father != null && father.Father.Age > 0)
                    return horse.Age - father.Father.Age;
            }

            return -1;
        }
        public double CalculateGDGD(Horse horse)
        {
            var father = horse.Father;
            if (father != null)
            {
                if (father.Mother != null && father.Mother.Age > 0)
                    return horse.Age - father.Mother.Age;
            }

            return -1;
        }
        public double CalculateGSSD(Horse horse)
        {
            var mother = horse.Mother;
            if (mother != null)
            {
                if (mother.Father != null && mother.Father.Age > 0)
                    return horse.Age - mother.Father.Age;
            }

            return -1;
        }
        public double CalculateGSDD(Horse horse)
        {
            var mother = horse.Mother;
            if (mother != null)
            {
                if (mother.Mother != null && mother.Mother.Age > 0)
                    return horse.Age - mother.Mother.Age;
            }

            return -1;
        }

        public double Percentile(IEnumerable<double> seq, double percentile)
        {
            var elements = seq.ToArray();
            Array.Sort(elements);
            double realIndex = percentile * (elements.Length - 1);
            int index = (int)realIndex;
            double frac = realIndex - index;
            if (index + 1 < elements.Length)
                return elements[index] * (1 - frac) + elements[index + 1] * frac;
            else
                return elements[index];
        }
    }

    internal class CommonHorsesComparer : IEqualityComparer<Horse>
    {
        public bool Equals(Horse x, Horse y)
        {
            return x.Id.Equals(y.Id);
        }

        public int GetHashCode(Horse obj)
        {
            unchecked
            {
                var hash = 17;
                //same here, if you only want to get a hashcode on a, remove the line with b
                hash = hash * 23 + obj.Id.GetHashCode();
                hash = hash * 23 + obj.Id.GetHashCode();

                return hash;
            }
        }
    }
    internal class CommonAncestorsIdComparer : IEqualityComparer<int>
    {
        public bool Equals(int x, int y)
        {
            return x == y;
        }

        public int GetHashCode(int obj)
        {
            unchecked
            {
                var hash = 17;
                //same here, if you only want to get a hashcode on a, remove the line with b
                hash = hash * 23 + obj.GetHashCode();
                hash = hash * 23 + obj.GetHashCode();

                return hash;
            }
        }

    }
}
