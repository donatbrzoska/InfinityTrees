using System;
public interface Grower : GrowthPropertiesListener {
    void Apply(Node node, bool regrow = true);
    GrowthProperties GetGrowthProperties();

    void Stop();

    void SetGrowerListener(GrowerListener growerListener);
}