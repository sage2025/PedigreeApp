using Pedigree.Core.Data.Entity.InMemoryOnly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pedigree.Core.Service
{
    public class PEDIG
    {
        private static readonly int NIM = 50;
        private static readonly double EPS = 0.00000001;
        private static readonly int NT = 2000000;


        public static Dictionary<int, double> Meuw(HorsePedigree pedigree)
        {
            int nl = pedigree.Count;

            int i, j, k;

            int[] sire = new int[nl], dam = new int[nl], ped1 = new int[nl], ped2 = new int[nl], point = new int[nl];
            int[] ord = new int[nl], rord = new int[nl];
            double[] d = new double[nl], f = new double[nl], l = new double[nl];
            int nbit;

            int[] ids = pedigree.Ids.ToArray();
            int[] sire1 = pedigree.Sire;
            int[] dam1 = pedigree.Dam;
            for (i = 0; i < pedigree.Count; i++)
            {
                sire[i] = sire1[i];
                dam[i] = dam1[i];
            }


            nbit = 0;
            for (i = 0; i < nl; i++) ord[i] = -1;

            k = 0;
            while (k < nl && nbit <= 50)
            {
                nbit++;
                j = 0;
                for (i = 0; i < nl; i++)
                {
                    if (ord[i] == -1)
                    {
                        if (sire[i] == -1 || ord[sire[i]] != -1)
                        {
                            if (dam[i] == -1 || ord[dam[i]] != -1)
                            {
                                ord[i] = k;
                                rord[k] = i;
                                k++;
                            }
                        }
                    }
                }
            }

            j = 0;
            if (k != nl)
            {
                for (i = 0; i < nl; i++)
                {
                    if (ord[i] == -1)
                    {
                        j++;
                        sire[i] = -1;
                        dam[i] = -1;
                        ord[i] = k;
                        rord[k] = i;
                        k++;
                    }
                }
            }

            for (i = 0; i < nl; i++)
            {
                j = rord[i];
                ped1[i] = sire[j];
                ped2[i] = dam[j];
                if (ped1[i] != -1) ped1[i] = ord[sire[j]];
                if (ped2[i] != -1) ped2[i] = ord[dam[j]];
            }

            for (i = 0; i < nl; i++) point[i] = -1;

            int[] npp = new int[200];
            for (i = 0; i < 200; i++) npp[i] = 0;

            int ninbr = 0;

            for (i = 0; i < nl; i++)
            {
                int si = ped1[i];
                int di = ped2[i];
                ped1[i] = Math.Max(si, di);
                ped2[i] = Math.Min(si, di);
                double fs = si == -1 ? -1 : f[si];
                double fd = di == -1 ? -1 : f[di];
                d[i] = 0.5 - 0.25 * (fs + fd);
                if (si == -1 || di == -1)
                {
                    f[i] = 0;
                }
                else
                {
                    int np = 0;
                    double fi = -1;
                    l[i] = 1;
                    j = i;
                    while (j != -1)
                    {
                        k = j;
                        double r = 0.5 * l[k];
                        int ks = ped1[k];
                        int kd = ped2[k];
                        if (ks > -1)
                        {
                            l[ks] += r;
                            while (point[k] > ks) k = point[k];
                            if (ks != point[k])
                            {
                                point[ks] = point[k];
                                point[k] = ks;
                            }
                            if (kd > -1)
                            {
                                l[kd] += r;
                                while (point[k] > kd) k = point[k];
                                if (kd != point[k])
                                {
                                    point[kd] = point[k];
                                    point[k] = kd;
                                }
                            }
                        }
                        fi += l[j] * l[j] * d[j];
                        l[j] = 0;
                        k = j;
                        j = point[j];
                        point[k] = -1;
                        np++;
                    }
                    f[i] = fi;
                    if (fi > 0.000001) ninbr++;
                    if (np > 199) np = 199;
                    if (np == 0)
                    {
                        Console.WriteLine("Error");
                        break;
                    }
                    npp[np]++;
                }
            }

            for (i = 0; i < nl; i++) d[rord[i]] = f[i];
            for (i = 0; i < nl; i++) f[i] = d[i];

            Dictionary<int, double> ret = new Dictionary<int, double>();
            for (i = 0; i < nl; i++)
            {
                if (f[i] < 0) f[i] = 0;
                ret[ids[i]] = Math.Round(f[i], 5);
            }

            return ret;
        }

        public static Dictionary<int, ProbOrigItem> ProbOrig(HorsePedigree pedigree, int nAncestors, int startYear, int endYear)
        {
            Dictionary<int, ProbOrigItem> ret = new Dictionary<int, ProbOrigItem>();

            int nl = pedigree.Count;
            int[] prc = new int[nl], mrc = new int[nl], ndes = new int[nl], ord = new int[nl], ngen = new int[nl];
            bool[] pop = new bool[nl];
            int[] indi = new int[1000];
            double[] prob = new double[nl], pro = new double[nl], aj = new double[nl];
            double x, y, cum, r, z, sum, xmin, xmaj;
            int i, j, k, l, nbit, ip, im, ii, npop, ng1, ng2, nfond, ngg, nr;

            int[] sire = pedigree.Sire;
            int[] dam = pedigree.Dam;
            for (i = 0; i < pedigree.Count; i++)
            {
                prc[i] = sire[i];
                mrc[i] = dam[i];
            }

            // initialisations
            for (i = 0; i < nl; i++)
            {
                prob[i] = 0;
                pop[i] = false;
                ndes[i] = 0;
            }

            // compute number of progeny
            k = 0;
            for (i = 0; i < nl; i++)
            {
                if (prc[i] != -1) ndes[prc[i]] += 1;
                if (mrc[i] != -1) ndes[mrc[i]] += 1;
            }
            for (i = 0; i < nl; i++) if (ndes[i] == 0) k++;

            // onstruction de ngen et de ord
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

            // we assume that the complete pedigree is ordered in less than 50 loops !
            nbit = 0;
            while (k < nl && nbit <= 50)
            {
                nbit++;
                j = 0;
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
                            ii = ip == -1 ? 0 : ngen[ip];
                            if (im != -1) ii = Math.Max(ii, ngen[im]);
                            ngen[i] = ii + 1;
                            ord[k] = i;
                            k++;
                            j++;
                        }
                    }
                }
            }

            // Check for pedigree
            j = 0;
            if (k < nl)
            {
                for (i = 0; i < nl; i++)
                {
                    if (ngen[i] < 0)
                    {
                        j++;
                        prc[i] = -1;
                        mrc[i] = -1;
                        ngen[i] = 0;
                        ord[k] = i;
                        k++;
                    }
                }
            }

            // define ref population
            npop = 0;
            ng1 = 0;
            ng2 = 0;
            for (i = 0; i < nl; i++)
            {
                var horse = pedigree.GetHorseByIndex(i);
                if (/*horse.Sex.Equals("Male") && */horse.Age >= startYear && horse.Age <= endYear && prc[i] != -1 && mrc[i] != -1)
                {
                    prob[i] = 1;
                    pop[i] = true;
                    npop++;
                    ng1 = Math.Max(ng1, ngen[i]);
                }
                else
                {
                    ng2 = Math.Max(ng2, ngen[i]);
                }
            }
            ngg = Math.Max(ng1, ng2);

            // calcul des probas d'origine des genes avec les fondateurs initiaux
            // on traite d'abord la population de depart
            for (k = nl - 1; k >= 0; k--)
            {
                i = ord[k];
                if (pop[i])
                {
                    ip = prc[i];
                    im = mrc[i];
                    if (ip != -1) prob[ip] += prob[i] * 0.5;
                    if (im != -1) prob[im] += prob[i] * 0.5;
                }
            }

            // on traite ensuite les autres
            for (k = nl - 1; k >= 0; k--)
            {
                i = ord[k];
                if (!pop[i])
                {
                    ip = prc[i];
                    im = mrc[i];
                    if (ip != -1) prob[ip] += prob[i] * 0.5;
                    if (im != -1) prob[im] += prob[i] * 0.5;
                }
            }

            x = 0;
            nfond = 0;
            for (i = 0; i < nl; i++)
            {
                if (prc[i] == -1 && mrc[i] == -1 && !pop[i] && prob[i] > 0)
                {
                    nfond++;
                    y = prob[i] / npop;
                    x += y * y;
                }
                else if ((prc[i] == -1 || mrc[i] == -1) && !pop[i] && prob[i] > 0)
                {
                    nfond++;
                    y = prob[i] * 0.5 / npop;
                    x += y * y;
                }
            }

            x = 1 / x;

            // recherche des nim individus les importants
            cum = 0;
            sum = 0;
            // transfert de prob dans pro
            for (i = 0; i < nl; i++) pro[i] = prob[i];

            // $$$$$$$$$$$$   analyse du lme fondateur $$$$$$$$$$$$$$$$$$$$
            l = 0;
            while (l < nAncestors && cum < 1 - EPS)
            {
                l++;

                // a:recherche de pro maxi
                x = 0;
                for (i = 0; i < l - 1; i++)
                {
                    pro[indi[i]] = 0;
                }
                for (i = 0; i < nl; i++)
                {
                    if (x < pro[i] && !pop[i]) // ancetre en dehors de la pop ref
                    {
                        j = i;
                        x = pro[i];
                    }
                }

                // calcul des majorant et minorant
                x = prob[j] / npop;
                y = pro[j] / npop;
                cum += y;
                sum += y * y;
                indi[l - 1] = j;
                r = y;
                if (r > 0)
                {
                    nr = (int)((1 - cum) / r);
                    z = 1 - cum - nr * r;
                }
                else
                {
                    nr = 0;
                    z = 0;
                }
                xmin = 1 / (sum + nr * r * r + z * z);
                nr = nfond - (l - 1);
                r = (nr > 0) ? (1 - cum) / nr : 0;
                xmaj = 1 / (sum + nr * r * r);

                ProbOrigItem item = new ProbOrigItem();
                item.X = x;
                item.Y = y;
                item.Cum = cum;
                item.Xmin = xmin;
                item.Xmaj = xmaj;
                ret[pedigree.Ids[j]] = item;

                // b: sauvegarde, annulation de genealogie, reinitialisation de pro
                prc[j] = -1;
                mrc[j] = -1;

                // initialisation
                for (i = 0; i < nl; i++) pro[i] = pop[i] ? 1 : 0;

                // c: calcul des pro
                // on traite d'abord la population de depart
                for (k = nl - 1; k >= 0; k--)
                {
                    i = ord[k];
                    if (pop[i])
                    {
                        ip = prc[i];
                        im = mrc[i];
                        if (ip != -1) pro[ip] += pro[i] * 0.5;
                        if (im != -1) pro[im] += pro[i] * 0.5;
                    }
                }

                // on traite ensuite les autres
                for (k = nl - 1; k >= 0; k--)
                {
                    i = ord[k];
                    if (!pop[i])
                    {
                        ip = prc[i];
                        im = mrc[i];
                        if (ip != -1) pro[ip] += pro[i] * 0.5;
                        if (im != -1) pro[im] += pro[i] * 0.5;
                    }
                }

                // d: ajustements pour les ancetres deja choisis
                for (i = 0; i < nl; i++) aj[i] = 0;
                for (i = 0; i < l; i++) aj[indi[i]] = 1;
                for (k = 0; k < nl; k++)
                {
                    i = ord[k];

                    ip = prc[i];
                    if (ip != -1 && aj[ip] != 0) aj[i] += 0.5 * aj[ip];

                    im = mrc[i];
                    if (im != -1 && aj[im] != 0) aj[i] += 0.5 * aj[im];
                }
                for (i = 0; i < nl; i++) pro[i] *= 1 - aj[i];
            }

            return ret;
        }

        public static Dictionary<int, GrainItem> Grain(HorsePedigree pedigree, int[] ancestors, int nsim = 10)
        {
            Dictionary<int, GrainItem> ret = new Dictionary<int, GrainItem>();

            int[] prc, mrc, ngen, ord, lf, listf;
            int[,] all, al, flag, allp, alp, flags;
            int[,,] flag1, flags1;
            double[] anc, inb, ancs, anck, fn;
            double[,] inbp, ancp, anckp, ancsp;

            int nt;
            int nl = pedigree.Count;
            int nlx = 0;
            int nf = ancestors.Length;

            int i, j, k, nbit, ip, im, nall, nff;

            int[] oids = pedigree.Ids;
            int[] sire = pedigree.Sire;
            int[] dam = pedigree.Dam;
            for (i = 0; i < pedigree.Count; i++)
            {
                if ((sire[i] == -1 && dam[i] != -1) || (sire[i] != -1 && dam[i] == -1))
                {
                    nlx++;
                }
            }

            nt = nl + nlx;

            prc = new int[nt];
            mrc = new int[nt];
            ngen = new int[nt];
            ord = new int[nt];
            lf = new int[nt];
            listf = new int[nf];

            anc = new double[nt];
            inb = new double[nt];
            ancs = new double[nt];
            anck = new double[nt];
            fn = new double[nt];

            all = new int[2, nt];
            al = new int[2, nt];
            flag = new int[2, nt];
            allp = new int[2, nt];
            alp = new int[2, nt];
            flags = new int[2, nt];

            inbp = new double[nt, nf];
            ancp = new double[nt, nf];
            anckp = new double[nt, nf];
            ancsp = new double[nt, nf];

            flag1 = new int[2, nt, nf];
            flags1 = new int[2, nt, nf];

            for (i = 0; i < pedigree.Count; i++)
            {
                prc[i] = sire[i];
                mrc[i] = dam[i];
            }

            // extension of pedigree for animals with one known parent
            k = nl;
            for (i = 0; i < nl; i++)
            {
                if (prc[i] == -1 && mrc[i] > -1)
                {
                    prc[k] = -1;
                    mrc[k] = -1;
                    prc[i] = k;
                    k++;
                }
                else if (prc[i] > -1 && mrc[i] == -1)
                {
                    prc[k] = -1;
                    mrc[k] = -1;
                    mrc[i] = k;
                    k++;
                }
            }
            nl = k;

            for (i = 0; i < nl; i++) lf[i] = -1;

            nff = 0;
            foreach (var ancestor in ancestors)
            {
                i = Array.IndexOf(oids, ancestor);
                if (i > -1)
                {
                    listf[nff] = i;
                    prc[i] = -1;
                    mrc[i] = -1;
                    lf[i] = nff;
                    nff++;
                }
            }

            // alleles for base animals
            nall = 0;
            for (i = 0; i < nl; i++)
            {
                all[0, i] = 0;
                all[1, i] = 0;
                if (prc[i] == -1)
                {
                    nall++;
                    all[0, i] = nall;
                    allp[0, i] = i;
                }
                if (mrc[i] == -1)
                {
                    nall++;
                    all[1, i] = nall;
                    allp[1, i] = i;
                }
                if (prc[i] == -1 || mrc[i] == -1)
                {
                    if (lf[i] == -1)
                    {
                        if (nff < nf)
                        {
                            listf[nff] = i;
                            lf[i] = nff;
                            nff++;
                        }
                    }
                }
            }

            // ordering of animals - ngen=0 for founders, >0 for others
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
            while (k < nl && nbit <= 20)
            {
                nbit++;
                j = 0;
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
                            j++;
                        }
                    }
                }
            }

            // searching for errors in pedigrees
            j = 0;
            if (k != nl)
            {
                for (i = 0; i < nl; i++)
                {
                    if (ngen[i] < 0)
                    {
                        j++;
                        prc[i] = -1;
                        mrc[i] = -1;
                        ord[k] = i;
                        ngen[i] = 0;
                        k++;
                    }
                }
            }

            // setting vectors for all inbreeding coefficients to zero
            for (i = 0; i < nl; i++)
            {
                // total inbreeding coefficients
                anc[i] = 0;
                inb[i] = 0;
                ancs[i] = 0;
                anck[i] = 0;
                fn[i] = 0;

                // partial inbreeding coefficients except for fac
                for (j = 0; j < nff; j++)
                {
                    ancp[i, j] = 0;
                    inbp[i, j] = 0;
                    anckp[i, j] = 0;
                }
            }
            //Console.WriteLine($" Total Number of founders analysed for contributions : {nff}");

            // starting the simulation of gene drop from now on
            Random rand = new Random(42);
            for (int ii = 0; ii < nsim; ii++)
            {
                for (i = 0; i < nl; i++)
                {
                    al[0, i] = all[0, i];
                    al[1, i] = all[1, i];
                    alp[0, i] = allp[0, i];
                    alp[1, i] = allp[1, i];
                    flag[0, i] = 0;
                    flag[1, i] = 0;
                    flags[0, i] = 0;
                    flags[1, i] = 0;

                    for (j = 0; j < nff; j++)
                    {
                        flag1[0, i, j] = 0;
                        flag1[1, i, j] = 0;
                        flags1[0, i, j] = 0;
                        flags1[1, i, j] = 0;
                    }
                }

                for (int kk = 0; kk < nl; kk++) //beginning of individuals
                {
                    i = ord[kk];

                    if (prc[i] > -1)
                    {
                        double x = rand.NextDouble(); 
                        k = (x > 0.5) ? 1 : 0;

                        al[0, i] = al[k, prc[i]];
                        alp[0, i] = alp[k, prc[i]];
                        if (flag[k, prc[i]] >= 1)
                        {
                            flag[0, i] = 1;
                            flags[0, i] = flags[k, prc[i]];
                        }
                        for (j = 0; j < nff; j++)
                        {
                            // keep information on past IBD
                            if (flag1[k, prc[i], j] >= 1) flag1[0, i, j] = 1;
                            if (flags1[k, prc[i], j] >= 1) flags1[0, i, j] = flags1[k, prc[i], j];
                        }
                    }

                    if (mrc[i] > -1)
                    {
                        double x = rand.NextDouble(); 
                        k = (x > 0.5) ? 1 : 0;

                        al[1, i] = al[k, mrc[i]];
                        alp[1, i] = alp[k, mrc[i]];
                        if (flag[k, mrc[i]] >= 1)
                        {
                            flag[1, i] = 1;
                            flags[1, i] = flags[k, mrc[i]];
                        }
                        for (j = 0; j < nff; j++)
                        {
                            // keep information on past IBD
                            if (flag1[k, mrc[i], j] >= 1) flag1[1, i, j] = 1;
                            if (flags1[k, mrc[i], j] >= 1) flags1[1, i, j] = flags1[k, mrc[i], j];
                        }
                    }
                    // sum up number flagged alleles over repetitions
                    anc[i] += flag[0, i] + flag[1, i];
                    ancs[i] += flags[0, i] + flags[1, i];

                    for (j = 0; j < nff; j++)
                    {
                        int jj = listf[j];
                        if (alp[0, i] == jj)
                        {
                            ancp[i, j] += flag1[0, i, j];
                            ancsp[i, j] += flags1[0, i, j];
                            if (al[0, i] == al[1, i])
                            {
                                anckp[i, j] += Math.Max(flag1[0, i, j], flag1[1, i, j]);
                            }
                        }
                        if (alp[1, i] == jj)
                        {
                            ancp[i, j] += flag1[1, i, j];
                            ancsp[i, j] += flags1[1, i, j];
                            if (al[0, i] == al[1, i])
                            {
                                anckp[i, j] += Math.Max(flag1[1, i, j], flag1[0, i, j]);
                            }
                        }
                    }

                    if (al[0, i] == al[1, i])
                    {
                        inb[i]++;
                        anck[i] += Math.Max(flag[0, i], flag[1, i]);
                        fn[i] = 2 * inb[i] - anck[i];
                        // flag alleles which are IBD
                        flag[0, i] = 1;
                        flag[1, i] = 1;
                        flags[0, i]++;
                        flags[1, i]++;

                        for (j = 0; j < nff; j++)
                        {
                            int jj = listf[j];
                            if (alp[0, i] == jj)
                            {
                                flag1[0, i, j]++;
                                flag1[1, i, j]++;
                                flags1[0, i, j]++;
                                flags1[1, i, j]++;
                                inbp[i, j]++;
                            }
                        }
                    }
                } //end of individual
            } // end of replicate

            // *** END OF REPLICATE

            // writing pedigree, ancestral inbreeding coefficient and inbreeding coefficient into output file
            for (k = 0; k < nl; k++)
            {
                i = ord[k];
                anc[i] /= 2 * nsim;     // ancestral Ballou (1997)
                inb[i] /= nsim;         // classical inbreeding coeff.
                ancs[i] /= 2 * nsim;    // ancestral Baumung et al. (2006)
                fn[i] /= 2 * nsim;      // new inbreeding Kalinowski et al. (2000)
                anck[i] /= nsim;        // ancestral Kalinowski et al. (2000)

                for (j = 0; j < nff; j++)
                {
                    inbp[i, j] /= nsim;         // partial classical
                    ancp[i, j] /= 2 * nsim;     // partial Ballou
                    ancsp[i, j] /= 2 * nsim;    // partial Baumung et al.
                    anckp[i, j] /= 2 * nsim;    // partial Kalinowski et al.
                }
            }

            for (k = 0; k < nl; k++)
            {
                i = ord[k];

                if (i < pedigree.Count)
                {
                    GrainItem item = new GrainItem();
                    item.HorseId = oids[i];
                    item.Inb = inb[i];
                    item.Anc = anc[i];
                    item.Ancs = ancs[i];
                    item.Anck = anck[i];

                    for (j = 0; j < nff; j++)
                    {
                        item.SetInbp(oids[listf[j]], inbp[i, j]);
                        item.SetAncp(oids[listf[j]], ancp[i, j]);
                        item.SetAncsp(oids[listf[j]], ancsp[i, j]);
                        item.SetAnckp(oids[listf[j]], anckp[i, j]);
                    }
                    ret[oids[i]] = item;
                }
            }
            return ret;
        }
        /*public void AncComm(PedItem[] pedigree, string[] horseIds)
        {
            int nl = pedigree.Length, npop = horseIds.Length;
            int[] prc = new int[nl], mrc = new int[nl], ndes = new int[nl], ngen = new int[nl], list = new int[nl], listp = new int[nl], ord = new int[nl];
            int i, j, k, npopp, necr, necrp, nan, ic, icm, icp, icmp, nbit, ip, im;
            bool[] pop = new bool[nl];
            double[] prob = new double[nl];
            double x;

            string[] ids = pedigree.Select(p => p.HorseId).ToArray();
            for (i = 0; i < nl; i++)
            {
                var item = pedigree[i];
                prc[i] = item.FatherId != null ? Array.IndexOf(ids, item.FatherId) : -1;
                mrc[i] = item.MotherId != null ? Array.IndexOf(ids, item.MotherId) : -1;
            }

            // lecture du fichier etudie
            npop = 0;   // nb d individus analyses
            npopp = 0;  // nb de parents des individus analyses
            for (i = 0; i < nl; i++)
            {
                pop[i] = false;     // individus analyses
                ndes[i] = 0;        // nbre de produits
            }

            foreach(var id in horseIds)
            {
                i = Array.IndexOf(ids, id);
                pop[i] = true;
                list[npop] = i;
                npop++;

                if (prc[i] > -1)
                {
                    listp[npopp] = prc[i];
                    npopp++;
                }
                if (mrc[i] > -1)
                {
                    listp[npopp] = mrc[i];
                    npopp++;
                }
            }

            Console.WriteLine($"Number of individuals studied         : {npop}");
            icm = npop * 4 / 5;
            if (npop < 5) icm = npop;

            // tri des parents et suppression des doubles
            // call ipsort(listp, npopp, ord,2, ifail)

            j = 0;
            for (i = 1; i < npopp; i++)
            {
                if (listp[i] > listp[j])
                {
                    j++;
                    listp[j] = listp[i];
                }
            }
            npopp = j;
            Console.WriteLine($"Nombre de parents des individus etudies : {npopp}");
            icmp = npopp * 4 / 5;
            if (npopp < 5) icmp = npopp;

            // calcul du nbre de descendants
            for (i = 0; i < nl; i++)
            {
                if (prc[i] != -1) ndes[prc[i]]++;
                if (mrc[i] != -1) ndes[mrc[i]]++;
            }

            // recherche de l ordre tel que parent est avant produit
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
            // on suppose que le pedigree est trie en moins de 50 boucles
            while(k < nl && nbit <= 50)
            {
                nbit++;
                j = 0;
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
                            j++;
                        }
                    }
                }
            }

            // verification que le pedigree est plausible
            j = 0;
            if (k < nl)
            {
                for(i = 0; i < nl; i++)
                {
                    if (ngen[i] < 0)
                    {
                        j++;
                        prc[i] = -1;
                        mrc[i] = -1;
                        ngen[i] = 0;
                        ord[k] = i;
                        k++;
                    }
                }
            }

            //  on calcule la contribution de chaque individu a tous ses descendants
            necr = 0; necrp = 0; nan = 0;
            for (int ii = 0; ii < nl; ii++)
            {
                if (ndes[ii] > 0)
                {
                    for (i = 0; i < nl; i++) prob[i] = 0;
                    prob[ii] = 1;
                    for (k = 0; k < nl; k++)
                    {
                        i = ord[k];
                        ip = prc[i];
                        im = mrc[i];
                        if (ip != -1) prob[i] += prob[ip] * 0.5;
                        if (im != -1) prob[i] += prob[im] * 0.5;
                    }
                    ic = 0;
                    x = 0;

                    // on determine ensuite la contribution de cet ancetre a chaque individu de ref
                    // on l ecrit dans sortie s il contribue a au moins icm individus
                    for (i = 0; i < npop; i++)
                    {
                        j = list[i];
                        if (prob[j] > 0)
                        {
                            ic++;
                            x += prob[j];
                        }
                    }
                    if (ic >= icm)
                    {
                        necr
                    }
                }
            }
        }*/


        public static List<Par3Item> Par3(HorsePedigree pedigree, int[] group1, int[] group2)
        {
            List<Par3Item> ret = new List<Par3Item>();
            int n = pedigree.Count;

            int i, j, k, ii, ip, im, nbit, ks, kd;
            int ig, ig1, ig2;
            int[] iopt = new int[4], nlist = new int[2];

            int[,] lliste = new int[10000, 2];


            int[] pere = new int[NT], mere = new int[NT], point = new int[NT];
            int[] ord = new int[NT], rord = new int[NT], id = new int[NT], ic = new int[NT], ndes = new int[NT];
            int[,] ped = new int[2, NT];
            double[] statis = new double[106];
            double x;
            double[] f = new double[NT], l = new double[NT], d = new double[NT];
            bool[,] ts2 = new bool[2, 2];

            for (i = 0; i < 2; i++)
                for (j = 0; j < 2; j++)
                    ts2[i, j] = true;


            nlist[0] = 0;
            nlist[1] = 0;

            int[] oids = pedigree.Ids;

            foreach (var aid in group1)
            {
                i = Array.IndexOf(oids, aid);
                lliste[nlist[0], 0] = i; nlist[0]++;
            }

            foreach (var aid in group2)
            {
                i = Array.IndexOf(oids, aid);
                lliste[nlist[1], 1] = i; nlist[1]++;
            }

            // lecture du pedigree
            int[] sire = pedigree.Sire;
            int[] dam = pedigree.Dam;
            for (i = 0; i < pedigree.Count; i++)
            {
                pere[i] = sire[i];
                mere[i] = dam[i];
            }


            // *** renumerotation du plus vieux au plus jeune
            nbit = 0;
            for (i = 0; i < n; i++) ord[i] = -1;

            k = 0;
            while (k < n && nbit <= 20)
            {
                nbit++;
                for (i = 0; i < n; i++)
                {
                    if (ord[i] == -1)
                    {
                        if (pere[i] == -1 || ord[pere[i]] != -1)
                        {
                            if (mere[i] == -1 || ord[mere[i]] != -1)
                            {
                                ord[i] = k;
                                rord[k] = i;
                                k++;
                            }
                        }
                    }
                }
            }


            // Test qu'il n'y a pas de boucle dans le pedigree
            j = 0;
            if (k != n)
            {
                for (i = 0; i < n; i++)
                {
                    if (ord[i] == -1)
                    {
                        j++;
                        pere[i] = -1;
                        mere[i] = -1;
                        ord[i] = k;
                        rord[k] = i;
                        k++;
                    }
                }
            }

            for (i = 0; i < n; i++)
            {
                j = rord[i];
                ped[0, i] = pere[j];
                ped[1, i] = mere[j];
                if (ped[0, i] > -1) ped[0, i] = ord[pere[j]];
                if (ped[1, i] > -1) ped[1, i] = ord[mere[j]];
            }

            for (i = 0; i < n; i++)
            {
                ks = ped[0, i];
                kd = ped[1, i];
                ped[0, i] = Math.Max(ks, kd);
                ped[1, i] = Math.Min(ks, kd);
            }

            for (ig = 0; ig < 2; ig++)
            {
                for (i = 0; i < nlist[ig]; i++)
                {
                    ndes[ord[lliste[i, ig]]] = 1;
                }
            }

            for (i = 0; i < n; i++)
            {
                point[i] = -1;
                l[i] = 0;
                d[i] = 0;
            }

            // Meuw routine
            int ninbr = 0;
            for (i = 0; i < n; i++)
            {
                if (ped[0, i] > -1) ndes[ped[0, i]] = ndes[ped[0, i]] + 1;
                if (ped[1, i] > -1) ndes[ped[1, i]] = ndes[ped[1, i]] + 1;
            }
            int npar = 0;
            int np;
            double fi;
            for (i = 0; i < n; i++)
            {
                if (ndes[i] > 0) npar++;
            }
            for (i = 0; i < n; i++)
            {
                if (ndes[i] > 0)
                {
                    ks = ped[0, i];
                    kd = ped[1, i];
                    ped[0, i] = Math.Max(ks, kd);
                    ped[1, i] = Math.Min(ks, kd);
                    double fs = ks == -1 ? -1 : f[ks];
                    double fd = kd == -1 ? -1 : f[kd];
                    d[i] = 0.5 - 0.25 * (fs + fd);
                    if (ks == -1 || kd == -1) f[i] = 0;
                    else
                    {
                        np = 0;
                        fi = -1;
                        l[i] = 1;
                        j = i;
                        while (j != -1)
                        {
                            k = j;
                            double r = 0.5 * l[k];
                            ks = ped[0, k];
                            kd = ped[1, k];
                            if (ks > -1)
                            {
                                l[ks] += r;
                                while (point[k] > ks) k = point[k];
                                if (ks != point[k])
                                {
                                    point[ks] = point[k];
                                    point[k] = ks;
                                }
                                if (kd > -1)
                                {
                                    l[kd] += r;
                                    while (point[k] > kd) k = point[k];
                                    if (kd != point[k])
                                    {
                                        point[kd] = point[k];
                                        point[k] = kd;
                                    }
                                }
                            }
                            fi += l[j] * l[j] * d[j];
                            l[j] = 0;
                            k = j;
                            j = point[j];
                            point[k] = -1;
                            np++;
                        }
                        f[i] = fi;
                        if (fi > 0.000001) ninbr++;
                    }
                }
            }


            n++;
            for (i = 0; i < n; i++)
            {
                point[i] = -1;
                l[i] = 0;
            }

            x = 0;
            for (ig1 = 0; ig1 < 2; ig1++)
            {
                for (ig2 = ig1; ig2 < 2; ig2++)
                {
                    Statt(x, statis, 0);

                    for (i = 0; i < nlist[ig1]; i++)
                    {
                        ii = 0;
                        if (ig1 == ig2) ii = i + 1;
                        for (j = ii; j < nlist[ig2]; j++)
                        {
                            ip = ord[lliste[i, ig1]];
                            im = ord[lliste[j, ig2]];
                            ped[0, n] = ip;
                            ped[1, n] = im;

                            Consang(n, ped, f, d, l, point);
                            if (ts2[ig1, ig2])
                            {
                                //Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", lliste[i, ig1], lliste[j, ig2], f[n], ig1, ig2);
                                if (ig1 != ig2)
                                {
                                    Par3Item item = new Par3Item();

                                    item.HorseId1 = oids[lliste[i, ig1]];
                                    item.HorseId2 = oids[lliste[j, ig2]];
                                    item.Coi = f[n];
                                    ret.Add(item);
                                }
                            }

                            Statt(f[n], statis, 1);
                        }
                    }

                    Statt(x, statis, 2);
                }
            }

            return ret;
        }


        private static void Statt(double x, double[] statis, int io)
        {
            int i, k;
            if (io == 0)
            {
                for (i = 0; i < 106; i++)
                {
                    statis[i] = 0;
                }
                return;
            }
            else if (io == 1)
            {
                statis[2] += 1;
                statis[3] += x;
                statis[4] += x * x;
                k = 5 + (int)(100 * x);
                statis[k] += 1;
                return;
            }
            else if (io == 2)
            {
                if (statis[2] > 0)
                {
                    statis[3] /= statis[2];
                    statis[4] -= statis[2] * statis[3] * statis[3];
                    statis[4] = Math.Sqrt(statis[4] / statis[2]);
                }
                for (i = 0; i < 100; i++)
                {
                    k = (int)(0.5 + 100 * statis[4 + i] / statis[2]);
                    if (statis[4 + i] > 0)
                    {

                    }
                }
            }
        }

        private static void Consang(int i, int[,] ped, double[] f, double[] d, double[] l, int[] point)
        {
            int ks = ped[0, i];
            int kd = ped[1, i];
            ped[0, i] = Math.Max(ks, kd);
            ped[1, i] = Math.Min(ks, kd);
            double fs = ks == -1 ? -1 : f[ks];
            double fd = kd == -1 ? -1 : f[kd];
            d[i] = 0.5 - 0.25 * (fs + fd);
            if (ks == -1 || kd == -1)
            {
                f[i] = 0;
                return;
            }
            double fi = -1;
            l[i] = 1;
            int j = i;
            while (j != -1)
            {
                int k = j;
                double r = 0.5 * l[k];
                ks = ped[0, k];
                kd = ped[1, k];
                if (ks > -1)
                {
                    l[ks] += r;
                    while (point[k] > ks) k = point[k];
                    if (ks != point[k])
                    {
                        point[ks] = point[k];
                        point[k] = ks;
                    }
                    if (kd > -1)
                    {
                        l[kd] += r;
                        while (point[k] > kd) k = point[k];
                        if (kd != point[k])
                        {
                            point[kd] = point[k];
                            point[k] = kd;
                        }
                    }
                }
                fi += l[j] * l[j] * d[j];
                l[j] = 0;
                k = j;
                j = point[j];
                point[k] = -1;
            }
            f[i] = fi;
        }
    }
}
