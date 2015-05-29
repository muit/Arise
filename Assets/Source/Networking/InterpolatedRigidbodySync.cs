using UnityEngine;
using System.Collections;

public class InterpolatedRigidbodySync : TNSyncRigidbody {
    public enum SmoothMethod {
        None,
        Interpolation,
        Extrapolation
    }

    public SmoothMethod method;


    /*
   Tension: 1 is high, 0 normal, -1 is low
   Bias: 0 is even,
         positive is towards first segment,
         negative towards the other
*/
    double HermiteInterpolate(
       double y0, double y1,
       double y2, double y3,
       double mu,
       double tension,
       double bias)
    {
        double m0, m1, mu2, mu3;
        double a0, a1, a2, a3;

        mu2 = mu * mu;
        mu3 = mu2 * mu;
        m0 = (y1 - y0) * (1 + bias) * (1 - tension) / 2;
        m0 += (y2 - y1) * (1 - bias) * (1 - tension) / 2;
        m1 = (y2 - y1) * (1 + bias) * (1 - tension) / 2;
        m1 += (y3 - y2) * (1 - bias) * (1 - tension) / 2;
        a0 = 2 * mu3 - 3 * mu2 + 1;
        a1 = mu3 - 2 * mu2 + mu;
        a2 = mu3 - mu2;
        a3 = -2 * mu3 + 3 * mu2;

        return (a0 * y1 + a1 * m0 + a2 * m1 + a3 * y2);
    }
}
