using System;
public interface Grower {
    //called by Core
    void Grow(Node node);
    void Stop();
}