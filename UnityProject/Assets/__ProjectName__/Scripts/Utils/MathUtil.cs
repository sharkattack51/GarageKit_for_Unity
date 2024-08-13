using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarageKit
{
    public class MathUtil
    {
        public static float LowPassFilter(float pre, float current, float weight = 0.05f)
        {
            return ((1.0f - weight) * pre) + (weight * current);
        }

#region Kalman
        public class KalmanFilter
        {
            /* usage
            https://garchiving.com/gyro-drift-correction/

            KalmanFilter kalmanX = new KalmanFilter();
            kalmanX.setAngle(roll); // Set starting angle
            kalAngleX = kalmanX.getAngle(angleAccel, gyroXrate, dt);

            angleAccel : 加速度センサーからの角度算出値(degree)
            gyroXrate : ジャイロセンサーの角速度(degree/s)
            dt : 時間(s)
            */

            // https://github.com/TKJElectronics/KalmanFilter

            /* Kalman filter variables */
            float Q_angle; // Process noise variance for the accelerometer
            float Q_bias; // Process noise variance for the gyro bias
            float R_measure; // Measurement noise variance - this is actually the variance of the measurement noise

            float angle; // The angle calculated by the Kalman filter - part of the 2x1 state vector
            float bias; // The gyro bias calculated by the Kalman filter - part of the 2x1 state vector
            float rate; // Unbiased rate calculated from the rate and the calculated bias - you have to call getAngle to update the rate

            float[,] P; // Error covariance matrix - This is a 2x2 matrix

            public KalmanFilter()
            {
                /* We will set the variables like so, these can also be tuned by the user */
                Q_angle = 0.001f;
                Q_bias = 0.003f;
                R_measure = 0.03f;

                angle = 0.0f; // Reset the angle
                bias = 0.0f; // Reset bias

                P = new float[2, 2];
                P[0, 0] = 0.0f; // Since we assume that the bias is 0 and we know the starting angle (use setAngle), the error covariance matrix is set like so - see: http://en.wikipedia.org/wiki/Kalman_filter#Example_application.2C_technical
                P[0, 1] = 0.0f;
                P[1, 0] = 0.0f;
                P[1, 1] = 0.0f;
            }

            public float GetAngle(float newAngle, float newRate, float dt)
            {
                // KasBot V2  -  Kalman filter module - http://www.x-firm.com/?page_id=145
                // Modified by Kristian Lauszus
                // See my blog post for more information: http://blog.tkjelectronics.dk/2012/09/a-practical-approach-to-kalman-filter-and-how-to-implement-it

                // Discrete Kalman filter time update equations - Time Update ("Predict")
                // Update xhat - Project the state ahead
                /* Step 1 */
                rate = newRate - bias;
                angle += dt * rate;

                // Update estimation error covariance - Project the error covariance ahead
                /* Step 2 */
                P[0, 0] += dt * ((dt * P[1, 1]) - P[0, 1] - P[1, 0] + Q_angle);
                P[0, 1] -= dt * P[1, 1];
                P[1, 0] -= dt * P[1, 1];
                P[1, 1] += Q_bias * dt;

                // Discrete Kalman filter measurement update equations - Measurement Update ("Correct")
                // Calculate Kalman gain - Compute the Kalman gain
                /* Step 4 */
                float S = P[0, 0] + R_measure; // Estimate error
                /* Step 5 */
                float[] K = new float[2]; // Kalman gain - This is a 2x1 vector
                K[0] = P[0, 0] / S;
                K[1] = P[1, 0] / S;

                // Calculate angle and bias - Update estimate with measurement zk (newAngle)
                /* Step 3 */
                float y = newAngle - angle; // Angle difference
                /* Step 6 */
                angle += K[0] * y;
                bias += K[1] * y;

                // Calculate estimation error covariance - Update the error covariance
                /* Step 7 */
                float P00_temp = P[0, 0];
                float P01_temp = P[0, 1];

                P[0, 0] -= K[0] * P00_temp;
                P[0, 1] -= K[0] * P01_temp;
                P[1, 0] -= K[1] * P00_temp;
                P[1, 1] -= K[1] * P01_temp;

                return angle;
            }

            public void SetAngle(float angle) { this.angle = angle; } // Used to set angle, this should be set as the starting angle
            public float GetRate() { return this.rate; } // Return the unbiased rate

            /* These are used to tune the Kalman filter */
            public void SetQangle(float Q_angle) { this.Q_angle = Q_angle; }
            public void SetQbias(float Q_bias) { this.Q_bias = Q_bias; }
            public void SetRmeasure(float R_measure) { this.R_measure = R_measure; }

            public float GetQangle() { return this.Q_angle; }
            public float GetQbias() { return this.Q_bias; }
            public float GetRmeasure() { return this.R_measure; }
        }
#endregion
    }
}
