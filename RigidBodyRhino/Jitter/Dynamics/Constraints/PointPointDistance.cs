﻿/* Copyright (C) <2009-2011> <Thorben Linneweber, Jitter Physics>
* 
*  This software is provided 'as-is', without any express or implied
*  warranty.  In no event will the authors be held liable for any damages
*  arising from the use of this software.
*
*  Permission is granted to anyone to use this software for any purpose,
*  including commercial applications, and to alter it and redistribute it
*  freely, subject to the following restrictions:
*
*  1. The origin of this software must not be misrepresented; you must not
*      claim that you wrote the original software. If you use this software
*      in a product, an acknowledgment in the product documentation would be
*      appreciated but is not required.
*  2. Altered source versions must be plainly marked as such, and must not be
*      misrepresented as being the original software.
*  3. This notice may not be removed or altered from any source distribution. 
*/

#region Using Statements
using System;
using System.Collections.Generic;

using Jitter.Dynamics;
using Jitter.LinearMath;
using Jitter.Collision.Shapes;
#endregion

namespace Jitter.Dynamics.Constraints
{
    /// <summary>
    /// The distance between two given points on two bodies will not
    /// exceed a value.
    /// </summary>
    public class PointPointDistance : Constraint
    {
        public enum DistanceBehavior
        {
            LimitDistance,
            LimitMaximumDistance,
            LimitMinimumDistance,
        }

        private JVector localAnchor1, localAnchor2;
        private JVector r1, r2;

        private float biasFactor = 0.1f;
        private float softness = 0.01f;
        private float distance;

        private DistanceBehavior behavior = DistanceBehavior.LimitDistance;

        /// <summary>
        /// Initializes a new instance of the DistanceConstraint class.
        /// </summary>
        /// <param name="body1">The first body.</param>
        /// <param name="body2">The second body.</param>
        /// <param name="anchor1">The anchor point of the first body in world space. 
        /// The distance is given by the initial distance between both anchor points.</param>
        /// <param name="anchor2">The anchor point of the second body in world space.
        /// The distance is given by the initial distance between both anchor points.</param>
        public PointPointDistance(RigidBody body1, RigidBody body2, JVector anchor1,JVector anchor2)
            : base(body1, body2)
        {
            JVector.Subtract(ref anchor1, ref body1.position, out localAnchor1);
            JVector.Subtract(ref anchor2, ref body2.position, out localAnchor2);

            JVector.Transform(ref localAnchor1, ref body1.invOrientation, out localAnchor1);
            JVector.Transform(ref localAnchor2, ref body2.invOrientation, out localAnchor2);

            distance = (anchor1 - anchor2).Length();//per calcolare la distanza calcola la lunghezza della diagonale
        }

        public float AppliedImpulse { get { return accumulatedImpulse; } }

        /// <summary>
        /// 
        /// </summary>
        public float Distance { get { return distance; } set { distance = value; } }

        /// <summary>
        /// 
        /// </summary>
        public DistanceBehavior Behavior { get { return behavior; } set { behavior = value; } }

        /// <summary>
        /// The anchor point of body1 in local (body) coordinates.
        /// </summary>
        public JVector LocalAnchor1 { get { return localAnchor1; } set { localAnchor1 = value; } }

        /// <summary>
        /// The anchor point of body2 in local (body) coordinates.
        /// </summary>
        public JVector LocalAnchor2 { get { return localAnchor2; } set { localAnchor2 = value; } }

        /// <summary>
        /// Defines how big the applied impulses can get.
        /// </summary>
        public float Softness { get { return softness; } set { softness = value; } }

        /// <summary>
        /// Defines how big the applied impulses can get which correct errors.
        /// </summary>
        public float BiasFactor { get { return biasFactor; } set { biasFactor = value; } }

        float effectiveMass = 0.0f;
        float accumulatedImpulse = 0.0f;
        float bias;
        float softnessOverDt;
        
        JVector[] jacobian = new JVector[4];

        bool skipConstraint = false;

        /// <summary>
        /// Called once before iteration starts.
        /// </summary>
        /// <param name="timestep">The 5simulation timestep</param>
        public override void PrepareForIteration(float timestep)
        {
            JVector.Transform(ref localAnchor1, ref body1.orientation, out r1);
            JVector.Transform(ref localAnchor2, ref body2.orientation, out r2);

            JVector p1, p2, dp;
            JVector.Add(ref body1.position, ref r1, out p1);
            JVector.Add(ref body2.position, ref r2, out p2);

            JVector.Subtract(ref p2, ref p1, out dp);

            float deltaLength = dp.Length() - distance;

            if (behavior == DistanceBehavior.LimitMaximumDistance && deltaLength <= 0.0f)
            {
                skipConstraint = true;
            }
            else if (behavior == DistanceBehavior.LimitMinimumDistance && deltaLength >= 0.0f)
            {
                skipConstraint = true;
            }
            else
            {
                skipConstraint = false;

                JVector n = p2 - p1;
                if (n.LengthSquared() != 0.0f) n.Normalize();

                jacobian[0] = -1.0f * n;
                jacobian[1] = -1.0f * (r1 % n);
                jacobian[2] = 1.0f * n;
                jacobian[3] = (r2 % n);

                effectiveMass = body1.inverseMass + body2.inverseMass
                    + JVector.Transform(jacobian[1], body1.invInertiaWorld) * jacobian[1]
                    + JVector.Transform(jacobian[3], body2.invInertiaWorld) * jacobian[3];

                softnessOverDt = softness / timestep;
                effectiveMass += softnessOverDt;

                effectiveMass = 1.0f / effectiveMass;

                bias = deltaLength * biasFactor * (1.0f / timestep);

                if (!body1.isStatic)
                {
                    body1.linearVelocity += body1.inverseMass * accumulatedImpulse * jacobian[0];
                    body1.angularVelocity += JVector.Transform(accumulatedImpulse * jacobian[1], body1.invInertiaWorld);
                }

                if (!body2.isStatic)
                {
                    body2.linearVelocity += body2.inverseMass * accumulatedImpulse * jacobian[2];
                    body2.angularVelocity += JVector.Transform(accumulatedImpulse * jacobian[3], body2.invInertiaWorld);
                }
            }
            
        }

        /// <summary>
        /// Iteratively solve this constraint.
        /// </summary>
        public override void Iterate()
        {
            if (skipConstraint) return;

            float jv =
                body1.linearVelocity * jacobian[0] +
                body1.angularVelocity * jacobian[1] +
                body2.linearVelocity * jacobian[2] +
                body2.angularVelocity * jacobian[3];

            float softnessScalar = accumulatedImpulse * softnessOverDt;

            float lambda = -effectiveMass * (jv + bias + softnessScalar);

            if (behavior == DistanceBehavior.LimitMinimumDistance)
            {
                float previousAccumulatedImpulse = accumulatedImpulse;
                accumulatedImpulse = JMath.Max(accumulatedImpulse + lambda, 0);
                lambda = accumulatedImpulse - previousAccumulatedImpulse;
            }
            else if (behavior == DistanceBehavior.LimitMaximumDistance)
            {
                float previousAccumulatedImpulse = accumulatedImpulse;
                accumulatedImpulse = JMath.Min(accumulatedImpulse + lambda, 0);
                lambda = accumulatedImpulse - previousAccumulatedImpulse;
            }
            else
            {
                accumulatedImpulse += lambda;
            }

            if (!body1.isStatic)
            {
                body1.linearVelocity += body1.inverseMass * lambda * jacobian[0];
                body1.angularVelocity += JVector.Transform(lambda * jacobian[1], body1.invInertiaWorld);
            }

            if (!body2.isStatic)
            {
                body2.linearVelocity += body2.inverseMass * lambda * jacobian[2];
                body2.angularVelocity += JVector.Transform(lambda * jacobian[3], body2.invInertiaWorld);
            }
        }


        public override void DebugDraw(IDebugDrawer drawer)
        {
            drawer.DrawLine(body1.position + r1, body2.position + r2);
        }

    }
}
