using System;
public interface GeometryPropertiesObserver {
    void OnLeafTypeChanged();
    void OnLeavesPerNodeChanged();
    void OnLeavesEnabledChanged();
    void OnLeafSizeChanged();
}
