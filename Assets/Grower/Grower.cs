using System;
public interface Grower {
    //called by Core
    void Grow(Tree tree);
    void RegrowStem(Tree tree);
    void Stop();
    float GetTreeHeight();
}