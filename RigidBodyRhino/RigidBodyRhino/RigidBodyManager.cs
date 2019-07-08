using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Jitter.Dynamics;
using Rhino.Geometry;
using Rhino;
using Jitter;
using Jitter.Collision;
using Jitter.Collision.Shapes;
using Rhino.DocObjects;
using Jitter.LinearMath;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;

namespace RigidBodyRhino
{
    static class RigidBodyManager
    {
        public static List<List<RigidBody>> FramesRigidBodies { set; get; } = new List<List<RigidBody>>();

        //All logic rigid bodies to insert in the world and after RigidBodyRhinoCommand are in the world
        public static List<RigidBody> RigidBodies { set; get; } = new List<RigidBody>();

        //All guids of every shape in Rhino doc 
        public static List<Guid> GuidList { set; get; } = new List<Guid>();

        //All type of geometry in the Rhino doc with center in 0,0,0 and no rotation
        public static List<Brep> GeometryList { set; get; } = new List<Brep>();

        public static World World { set; get; } = new World(new CollisionSystemSAP());

        public static RigidBody DuplicateRigidBody(RigidBody rigidBody, bool isStatic)
        {
            RigidBody copyToAdd = new RigidBody(rigidBody.Shape)
            {
                Orientation = rigidBody.Orientation,
                Position = rigidBody.Position,
                Force = rigidBody.Force,
                LinearVelocity = rigidBody.LinearVelocity,
                AngularVelocity = rigidBody.AngularVelocity,
                Material = rigidBody.Material,
                Mass = rigidBody.Mass,
                BroadphaseTag = rigidBody.BroadphaseTag,
                Damping = rigidBody.Damping,
                EnableSpeculativeContacts = rigidBody.EnableSpeculativeContacts,
                IsParticle = rigidBody.IsParticle
            };
            
            if (isStatic)
            {
                copyToAdd.IsStatic = true;
            }
            return copyToAdd;
        }

        // Convert an object to a byte array
        public static byte[] ObjectToByteArray(Object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        // Convert a byte array to an Object
        public static Object ByteArrayToObject(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return obj;
            }
        }

        public static JVector Point3dtoJVector(Point3d point)
        {
            return new JVector((float)point.X, (float)point.Y, (float)point.Z);
        }

        public static Point3d JVectorToPoint3d(JVector vector)
        {
            return new Point3d(vector.X, vector.Y, vector.Z);
        }

        public static double ToRadians(double val)
        {
            return (Math.PI / 180) * val;
        }


        public static Transform MatrixTransfRotation(RigidBody body)
        {
            Transform trafo = Transform.Identity;
            trafo.M00 = body.Orientation.M11;
            trafo.M01 = body.Orientation.M12;
            trafo.M02 = body.Orientation.M13;
            //0
            trafo.M10 = body.Orientation.M21;
            trafo.M11 = body.Orientation.M22;
            trafo.M12 = body.Orientation.M23;
            //0
            trafo.M20 = body.Orientation.M31;
            trafo.M21 = body.Orientation.M32;
            trafo.M22 = body.Orientation.M33;
            //0
            return trafo;
        }

    }
}
