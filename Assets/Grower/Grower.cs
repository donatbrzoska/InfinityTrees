using System;
public interface Grower : GrowthPropertiesListener {
    void Apply(Node node);
    GrowthProperties GetGrowthProperties();

    void SetGrowerListener(GrowerListener growerListener);
}