import java.util.ArrayList;
import java.util.Random;

public class EA {
    public knapsack knapsack;

    public EA(knapsack knapsack) {
        this.knapsack = knapsack;
    }

    public int fitness(ArrayList<Boolean> solution) {
        int total_value = 0;
        int total_weight = 0;

        for (int i = 0; i < this.knapsack.items.size(); i++) {
            if (solution.get(i) == true) {
                total_value += this.knapsack.items.get(i).value;
                total_weight += this.knapsack.items.get(i).weight;
            }
        }

        if (total_weight <= this.knapsack.capacity) {
            return total_value;
        } else {
            return 0;
        }
    }

    public ArrayList<Boolean> mutate(ArrayList<Boolean> solution) {
        Random rd = new Random();
        ArrayList<Boolean> new_solution = solution;
        int index = rd.nextInt(solution.size() - 1);
        if (new_solution.get(index)) {
            new_solution.set(index, false);
        } else {
            new_solution.set(index, true);
        }

        return new_solution;
    }

    public ArrayList<Boolean> OPO(int tau) {
        ArrayList<Boolean> best = new ArrayList<Boolean>();
        for (int i = 0; i < this.knapsack.items.size(); i++) {
            best.set(i, false);
        }
        int best_fitness = this.fitness(best);

        for (int i = 0; i < tau; i++) {
            ArrayList<Boolean> child = mutate(best);
            int child_fitness = fitness(child);

            if (child_fitness > best_fitness) {
                best = child;
                best_fitness = child_fitness;
            }
        }
        return best;
    }
}