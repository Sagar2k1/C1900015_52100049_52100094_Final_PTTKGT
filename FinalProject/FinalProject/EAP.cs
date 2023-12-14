using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject
{
    class Invidual
    {
        public int Weight { get; set; }
        public int Value { get; set; }
        public bool isChoice { get; set; }

        public Invidual(int weight, int value, bool choice)
        {
            Weight = weight;
            Value = value;
            isChoice = choice;
        }
        public override string ToString()
        {
            return Weight + "" + Value + "" + isChoice;
        }
    }
    class Knapsack
    {
        private Invidual[] inviduals;
        private int capacity;

        public Knapsack(Invidual[] inviduals, int capacity)
        {
            this.inviduals = inviduals;
            this.capacity = capacity;
        }

        public int Capacity 
        {
            get 
            { 
                return capacity; 
            }
        }
        public void setCapacity(int capacity)
        {
            this.capacity = capacity;
        }
        public int TotalWeight
        {
            get
            {
                int total = 0;
                for(int i=0;i<inviduals.Length; i++)
                {
                    if (inviduals[i].isChoice)
                    {
                        total += inviduals[i].Weight;
                    }
                }
                return total;
            }
        }
        public int TotalValue
        {
            get
            {
                int totalValue = 0;
                for (int i = 0; i < inviduals.Length; i++)
                {
                    if (inviduals[i].isChoice)
                    {
                        totalValue += inviduals[i].Value;
                    }
                }
                return totalValue;
            }
        }
        public int Fitness
        {
            get
            {
                if(TotalWeight <capacity)
                {
                    return TotalValue;
                }
                return 0;
            }
        }
        public bool[] listChoice
        {
            get
            {
                bool[] choices = new bool[inviduals.Length];
                for(int i=0; i < inviduals.Length; i++)
                {
                    choices[i] = inviduals[i].isChoice;
                }
                return choices;
            }
        }
        public void setListChoices(bool[] choices)
        {
            for(int i = 0; i < inviduals.Length; i++)
            {
                inviduals[i].isChoice = choices[i];
            }
        }
        
    }
    
    

    


    public class EAP
    {
        static bool[] Mutate(bool[] listchoices, double mutate_rate)
        {
            Random rd = new Random();
            for (int i = 0; i < listchoices.Length; i++)
            {
                if (rd.NextDouble() < mutate_rate)
                {
                    if (listchoices[i])
                    {
                        listchoices[i] = false;
                    }
                    else
                    {
                        listchoices[i] = true;
                    }
                }
            }
            return listchoices;
        }
        
        
        internal static Knapsack OnePlusOne(Knapsack solution, int iter)
        {
            Random rd = new Random();
            for (int i = 0; i < iter; i++)
            {
                Knapsack y = solution;
                bool[] oldlist = solution.listChoice;
                bool[] M = Mutate(oldlist, rd.NextDouble());
                y.setListChoices(M);
                if (y.Fitness >= solution.Fitness)
                {
                    solution = y;
                }
            }
            return solution;
        }

        internal abstract class MOEA
        {
            public abstract bool GreaterEqualThan(Knapsack x, Knapsack y);
            public abstract bool GreaterThan(Knapsack x, Knapsack y);
            public bool CheckGreaterEqualThan(List<Knapsack> S, Knapsack y)
            {
                foreach (Knapsack p in S)
                {
                    if (GreaterEqualThan(p, y))
                    {
                        return false;
                    }
                }
                return true;
            }

            public List<List<Knapsack>> Repair(Knapsack q, int delta, int tau)
            {
                List<Knapsack> Splus = new List<Knapsack>();
                List<Knapsack> Sminus = new List<Knapsack>();
                int C = q.Capacity;
                Random rd = new Random();

                while (Splus.Count + Sminus.Count == 0)
                {
                    Knapsack y = q;
                    bool[] oldlist = q.listChoice;
                    bool[] M = Mutate(oldlist, rd.NextDouble());
                    y.setListChoices(M);
                    if (y.Fitness >= q.Fitness)
                    {
                        q = y;
                        if (q.TotalWeight > C && q.TotalWeight <= C + delta)
                        {
                            Splus.Add(q);
                        }
                        else if (q.TotalWeight >= C - delta && q.TotalWeight <= C)
                        {
                            Sminus.Add(q);
                        }
                    }
                }
                return new List<List<Knapsack>> { Splus, Sminus };
            }
            public Knapsack GetAlgorithm(Knapsack solution, int delta, int tau)
            {
                List<Knapsack> Splus = new List<Knapsack>();
                List<Knapsack> Sminus = new List<Knapsack>();
                Random rd = new Random();

                for (int i = 0; i < tau; i++)
                {
                    List<Knapsack> intersaction = new List<Knapsack>();
                    List<List<Knapsack>> temp = new List<List<Knapsack>>();

                    if (Splus.Count + Sminus.Count == 0)
                    {
                        temp = Repair(solution, delta, tau);
                        Splus = temp[0];
                        Sminus = temp[1];
                    }
                    else
                    {
                        foreach (Knapsack p in Splus)
                        {
                            intersaction.Add(p);
                        }
                        foreach (Knapsack p in Sminus)
                        {
                            intersaction.Add(p);
                        }
                        solution = intersaction[rd.Next(intersaction.Count)];

                        Knapsack y = solution;
                        bool[] oldlist = solution.listChoice;
                        bool[] M = Mutate(oldlist, rd.NextDouble());
                        y.setListChoices(M);
                        int C = solution.Capacity;
                        int totalYWeight = y.TotalWeight;
                        if (totalYWeight > C && totalYWeight <= C + delta && CheckGreaterEqualThan(Splus, y))
                        {
                            List<Knapsack> Splus1 = Splus;
                            Splus.Add(y);
                            foreach (Knapsack z in Splus1)
                            {
                                if (GreaterThan(z, y))
                                {
                                    Splus.Add(z);
                                }
                            }
                        }
                        if (totalYWeight >= C - delta && totalYWeight <= C && CheckGreaterEqualThan(Sminus, y))
                        {
                            List<Knapsack> Sminus1 = Sminus;
                            Splus.Add(y);
                            foreach (Knapsack z in Sminus)
                            {
                                if (GreaterThan(z, y))
                                {
                                    Splus.Add(z);
                                }
                            }
                        }


                    }
                }
                return solution;
            }
        }
        internal class BaseMOEA : MOEA
        {
            public override bool GreaterEqualThan(Knapsack x, Knapsack y)
            {
                if (y.TotalWeight == x.TotalWeight && x.Fitness >= y.Fitness) { return true; }
                return false;
            }

            public override bool GreaterThan(Knapsack x, Knapsack y)
            {
                if (y.TotalWeight == x.TotalWeight && x.Fitness > y.Fitness) { return true; }
                return false;
            }
        }
        internal class MOEAD : MOEA
        {
            public override bool GreaterEqualThan(Knapsack x, Knapsack y)
            {
                if (y.TotalWeight <= x.TotalWeight && x.TotalValue >= y.TotalValue) { return true; }
                return false;
            }

            public override bool GreaterThan(Knapsack x, Knapsack y)
            {
                if (y.TotalWeight < x.TotalWeight && x.TotalValue > y.TotalValue) { return true; }
                return false;
            }
        }

        

        internal class NSGA2
        {
            public List<Knapsack> GetOffstring(List<Knapsack> parents)
            {
                List<Knapsack> offstring = new List<Knapsack>();
                Random rd = new Random();
                for (int i = 0; i < parents.Count - 1; i++)
                {
                    for (int j = i + 1; j < parents.Count; j++)
                    {
                        Knapsack parent = parents[i], parent2 = parents[j];
                        parent.setCapacity(parent.Capacity + parent2.Capacity);
                        offstring.Add(parent);

                    }
                }
                return offstring;
            }
            public List<Knapsack> Combine(List<Knapsack> population, List<Knapsack> offstring)
            {
                foreach (Knapsack c in offstring)
                {
                    population.Add(c);
                }
                return population;
            }

            private List<List<Knapsack>> Repair(Knapsack q, int delta, int tau)
            {
                List<Knapsack> Splus = new List<Knapsack>();
                List<Knapsack> Sminus = new List<Knapsack>();
                int C = q.Capacity;
                Random rd = new Random();

                while (Splus.Count + Sminus.Count == 0)
                {
                    Knapsack y = q;
                    bool[] oldlist = q.listChoice;
                    bool[] M = Mutate(oldlist, rd.NextDouble());
                    y.setListChoices(M);
                    if (y.Fitness >= q.Fitness)
                    {
                        q = y;
                        if (q.TotalWeight > C && q.TotalWeight <= C + delta)
                        {
                            Splus.Add(q);
                        }
                        else if (q.TotalWeight >= C - delta && q.TotalWeight <= C)
                        {
                            Sminus.Add(q);
                        }
                    }
                }
                return new List<List<Knapsack>> { Splus, Sminus };
            }
            public List<Knapsack> NonDominateSort(List<Knapsack> list)
            {
                for(int i=0; i < list.Count; i++)
                {
                    for(int j=i+1; j < list.Count; j++)
                    {
                        if (list[i].Capacity<= list[j].Capacity)
                        {
                            Knapsack temp = list[i];
                            list[i] = list[j];
                            list[j] = temp;
                        }
                    }
                }
                return list;
            }
            public List<Knapsack> getAlgorithm(Knapsack solution, int delta, int tau)
            {
                int population_size = delta;
                Random rd = new Random();
                MOEAD mOEAD = new MOEAD();
                List<Knapsack> population = new List<Knapsack> ();
                for(int i = 0; i < population_size; i++)
                {
                    Knapsack s = solution;
                    bool[] oldlist = solution.listChoice;
                    bool[] M = Mutate(oldlist, rd.NextDouble());
                    s.setListChoices(M);
                    population.Add(s);

                }


                for(int i = 0; i < tau;i++)
                {
                    List<Knapsack> offstring = GetOffstring(population);
                    List<Knapsack> R = Combine(population, offstring);
                    List<Knapsack> front = NonDominateSort(R);
                    int j = 0;
                    List<Knapsack> new_population = new List<Knapsack> { front[0] };
                    for(j=0;j<population_size;j++)
                    {
                        new_population.Add(front[rd.Next(population_size)]);
                    }
                    population = new_population;
                }
                return population;
            }
        }

    }

}
