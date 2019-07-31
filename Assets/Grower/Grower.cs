using System;
public interface Grower {
    //called by Tree
    void Apply(Node node, bool regrow = true);
    GrowthProperties GetGrowthProperties();

    //called by Core
    void Stop();
    void SetGrowerListener(GrowerListener growerListener);
}