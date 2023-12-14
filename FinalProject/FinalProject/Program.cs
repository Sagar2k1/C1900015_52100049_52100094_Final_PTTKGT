// See https://aka.ms/new-console-template for more information
using FinalProject;

int capacity = 10;
int n = 200, r = 5, tau = 50;
int weight, value;
Random random = new Random();
Invidual[] inviduals = new Invidual[n];
for (int i = 0; i < n; i++)
{
    weight = random.Next(r);
    value = random.Next(r);
    int j = random.Next(2);
    if (j==0)
    {
        inviduals[i] = new Invidual(weight, value, true);
    }
    else
    {
        inviduals[i] = new Invidual(weight, value, false);
    }
}

Knapsack knapsack = new Knapsack(inviduals, capacity);
capacity = knapsack.TotalWeight+1;
knapsack = new Knapsack(inviduals, capacity);
Console.WriteLine("Before Fitness: " + knapsack.Fitness);

// One plus one
Knapsack knapsack1 = EAP.OnePlusOne(knapsack, tau);
Console.WriteLine("OnePlusOne Fitness: "+ knapsack1.Fitness);

// moea
EAP.BaseMOEA mOEA = new EAP.BaseMOEA();
Knapsack knapsack2 = mOEA.GetAlgorithm(knapsack, r, tau);
Console.WriteLine("Moea Fitness: " + knapsack2.Fitness);

// moead
EAP.MOEAD mOEAD = new EAP.MOEAD();
Knapsack knapsack3 = mOEAD.GetAlgorithm(knapsack, r, tau);
Console.WriteLine("Moea d Fitness: " + knapsack3.Fitness);
// nsga 2
EAP.NSGA2 nSGA2 = new EAP.NSGA2();
List<Knapsack> lknapsack4 = nSGA2.getAlgorithm(knapsack, r, tau);
Knapsack knapsack4 = lknapsack4[0];
Console.WriteLine("NSGA2 Fitness: " + knapsack4.TotalValue);

