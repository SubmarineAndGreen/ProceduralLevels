using System.Collections.Generic;

public class AdjacencyRules {

    HashSet<Rule> rules;

    public AdjacencyRules()
    {
        rules = new HashSet<Rule>();
    }

    public void allow(int tileA, int tileB, Directions3D directionBtoA) {
        rules.Add(new Rule(tileA, tileB, directionBtoA));
    }

    struct Rule {
        int tileA;
        int tileB;
        Directions3D directionBtoA;

        public Rule(int tileA, int tileB, Directions3D directionBtoA)
        {
            this.tileA = tileA;
            this.tileB = tileB;
            this.directionBtoA = directionBtoA;
        }
    }
}