import java.util.ArrayList;
import java.util.Arrays;
import java.util.Random;

public class MOEA {
    public knapsack knapsack;

    public MOEA(knapsack knapsack) {
        this.knapsack = knapsack;
    }

    public ArrayList<Integer> generate_individual() {
        ArrayList<Integer> individual = new ArrayList<>();
        Random rd = new Random();
        for (int i = 0; i < this.knapsack.items.size(); i++) {
            individual.add(rd.nextInt(2));
        }
        return individual;
    }

    public int evaluate(ArrayList<Integer> individual) {
        int total_weight = 0;
        for (int i = 0; i < this.knapsack.items.size(); i++) {
            total_weight += this.knapsack.items.get(i).weight;
        }
        int napsack_capacity = total_weight+1, total_value = 0;
        total_weight = 0;
        for (int i = 0; i < this.knapsack.items.size(); i++) {
            total_value += this.knapsack.items.get(i).value * individual.get(i);
            total_weight += this.knapsack.items.get(i).weight * individual.get(i);
        }

        if (total_weight > napsack_capacity) {
            total_value = 0;
        }

        return total_value;
    }
}