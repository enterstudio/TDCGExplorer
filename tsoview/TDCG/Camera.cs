using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace TDCG
{
    /// <summary>
    /// �J����
    /// </summary>
public class Camera
{
    private Vector3 center = Vector3.Empty;
    private Vector3 translation = Vector3.Empty;
    private Vector3 localP = new Vector3(0.0f, 0.0f, -10.0f);
    private Vector3 dirD = Vector3.Empty; //�J�����ړ������x�N�g��
    private float zD = 0.0f;      //�J�������s�I�t�Z�b�g�l
    private bool needUpdate = true;    //�X�V����K�v�����邩
    private Matrix view = Matrix.Identity;  //�r���[�s��
    private Matrix pose = Matrix.Identity;
    private float rotZD = 0.0f;   //�J���� Z����]����
    private float angleU = 0.02f;        //�ړ�����]�P�ʁi���W�A���j

    /// <summary>
    /// ��]���S
    /// </summary>
    public Vector3 Center { get { return center; } set { center = value; } }

    /// <summary>
    /// view���W��̃J�����̈ʒu
    /// </summary>
    public Vector3 Translation { get { return translation; } set { translation = value; } }

    /// <summary>
    /// �����_�����_�Ƃ������W��̃J�����̈ʒu
    /// </summary>
    public Vector3 LocalPosition { get { return localP; } set { localP = value; } }
    
    /// <summary>
    /// �J�����̎p���s��
    /// </summary>
    public Matrix Pose { get { return pose; } set { pose = value; } }

    /// <summary>
    /// �J�����𐶐����܂��B
    /// </summary>
    public Camera()
    {
        motion = new CameraMotion(this);
    }

    /// <summary>
    /// �J�����̈ʒu�Ǝp����W���o�͂֏����o���܂��B
    /// </summary>
    public void Dump()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Camera));
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Encoding = Encoding.GetEncoding("Shift_JIS");
        settings.Indent = true;
        XmlWriter writer = XmlWriter.Create(Console.Out, settings);
        serializer.Serialize(writer, this);
        writer.Close();
    }

    /// <summary>
    /// �J�����̈ʒu�Ǝp�����w��p�X�֏����o���܂��B
    /// </summary>
    /// <param name="dest_file">�p�X</param>
    public void Save(string dest_file)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Camera));
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Encoding = Encoding.GetEncoding("Shift_JIS");
        settings.Indent = true;
        XmlWriter writer = XmlWriter.Create(dest_file, settings);
        serializer.Serialize(writer, this);
        writer.Close();
    }

    /// <summary>
    /// �J�����̈ʒu�Ǝp�����w��p�X����ǂݍ��݂܂��B
    /// </summary>
    /// <param name="source_file">�p�X</param>
    /// <returns>�J����</returns>
    public static Camera Load(string source_file)
    {
        XmlReader reader = XmlReader.Create(source_file);
        XmlSerializer serializer = new XmlSerializer(typeof(Camera));
        Camera camera = serializer.Deserialize(reader) as Camera;
        reader.Close();
        return camera;
    }

    /// <summary>
    /// �J�����̈ʒu�Ǝp����⊮���܂��B
    /// </summary>
    /// <param name="cam1">��ԊJ�n���̈ʒu���p����ێ�����J����</param>
    /// <param name="cam2">��ԏI�����̈ʒu���p����ێ�����J����</param>
    /// <param name="ratio">��Ԕ䗦</param>
    /// <returns>�J����</returns>
    public static Camera Interpolation(Camera cam1, Camera cam2, float ratio)
    {
        Camera camera = new Camera();
        camera.Center = Vector3.Lerp(cam1.Center, cam2.Center, ratio);
        camera.Translation = Vector3.Lerp(cam1.Translation, cam2.Translation, ratio);
        camera.LocalPosition = Vector3.Lerp(cam1.LocalPosition, cam2.LocalPosition, ratio);
        Quaternion q1 = Quaternion.RotationMatrix(cam1.Pose);
        Quaternion q2 = Quaternion.RotationMatrix(cam2.Pose);
        camera.Pose = Matrix.RotationQuaternion(Quaternion.Slerp(q1, q2, ratio));
        return camera;
    }

    /// <summary>
    /// �J�����̈ʒu�Ǝp����⊮���܂��B
    /// </summary>
    /// <param name="cam1">��ԊJ�n���̈ʒu���p����ێ�����J����</param>
    /// <param name="cam2">��ԏI�����̈ʒu���p����ێ�����J����</param>
    /// <param name="ratio">��Ԕ䗦</param>
    public void Interp(Camera cam1, Camera cam2, float ratio)
    {
        center = Vector3.Lerp(cam1.Center, cam2.Center, ratio);
        translation = Vector3.Lerp(cam1.Translation, cam2.Translation, ratio);
        localP = Vector3.Lerp(cam1.LocalPosition, cam2.LocalPosition, ratio);
        Quaternion q1 = Quaternion.RotationMatrix(cam1.Pose);
        Quaternion q2 = Quaternion.RotationMatrix(cam2.Pose);
        pose = Matrix.RotationQuaternion(Quaternion.Slerp(q1, q2, ratio));

        //view�s��X�V
        Vector3 posW = localP + center;
        {
            Matrix m = pose;
            m.M41 = posW.X;
            m.M42 = posW.Y;
            m.M43 = posW.Z;
            m.M44 = 1.0f;
            view = Matrix.Invert(m) * Matrix.Translation(-translation);
        }

        //���������Z�b�g
        ResetDefValue();
        needUpdate = false;
    }

    /// <summary>
    /// �J�����̈ʒu�Ǝp�������Z�b�g���܂��B
    /// </summary>
    public void Reset()
    {
        center = Vector3.Empty;
        translation = Vector3.Empty;
        localP = new Vector3(0.0f, 0.0f, -10.0f);
        pose = Matrix.Identity;
        needUpdate = true;
    }

    /// <summary>
    /// �J�����̈ʒu���X�V���܂��B
    /// </summary>
    /// <param name="dirX">�ړ������i�o�x�j</param>
    /// <param name="dirY">�ړ������i�ܓx�j</param>
    /// <param name="dirZ">�ړ������i���s�j</param>
    public void Move(float dirX, float dirY, float dirZ)
    {
        if (dirX == 0.0f && dirY == 0.0f && dirZ == 0.0f)
            return;

        dirD.X += dirX;
        dirD.Y += dirY;
        this.zD += dirZ;
        needUpdate = true;
    }

    /// <summary>
    /// �J������Z����]���܂��B
    /// </summary>
    /// <param name="angle">��]�p�x�i���W�A���j</param>
    public void RotZ(float angle)
    {
        if (angle == 0.0f)
            return;

        rotZD = angle;
        needUpdate = true;
    }

    /// <summary>
    /// �J�����̈ʒu�Ǝp�����X�V���܂��B
    /// �}�E�X�̉�]���S�͌��_�Ƀ��Z�b�g���܂��B
    /// ���ӁF���̑���� Move() RotZ() Update() �Ƃ͈قȂ�n���ł��B
    /// </summary>
    /// <param name="eye">�����_</param>
    /// <param name="center">view���W��̃J�����̈ʒu</param>
    /// <param name="up">����x�N�g��</param>
    public void LookAt(Vector3 eye, Vector3 center, Vector3 up)
    {
        this.localP = center - eye;
        {
            // �J�����p�����X�V
            Vector3 z = Vector3.Normalize(-localP);
            Vector3 y = up;
            Vector3 x = Vector3.Normalize(Vector3.Cross(y, z));
            y = Vector3.Normalize(Vector3.Cross(z, x));
            {
                Matrix m = Matrix.Identity;
                m.M11 = x.X;
                m.M12 = x.Y;
                m.M13 = x.Z;
                m.M21 = y.X;
                m.M22 = y.Y;
                m.M23 = y.Z;
                m.M31 = z.X;
                m.M32 = z.Y;
                m.M33 = z.Z;
                this.pose = m;
            }
        }
        this.center = Vector3.Empty;
        this.translation = eye;

        //view�s��X�V
        Vector3 posW = localP + this.center;
        {
            Matrix m = pose;
            m.M41 = posW.X;
            m.M42 = posW.Y;
            m.M43 = posW.Z;
            m.M44 = 1.0f;
            view = Matrix.Invert(m) * Matrix.Translation(-translation);
        }

        //���������Z�b�g
        ResetDefValue();
        needUpdate = false;
    }

    /// <summary>
    /// �J�����̈ʒu�Ǝp�����X�V���܂��B
    /// �}�E�X�̉�]���S�͌��_�Ƀ��Z�b�g���܂��B
    /// ���ӁF���̑���� Move() RotZ() Update() �Ƃ͈قȂ�n���ł��B
    /// </summary>
    /// <param name="eye">�����_</param>
    /// <param name="center">view���W��̃J�����̈ʒu</param>
    public void LookAt(Vector3 eye, Vector3 center)
    {
        LookAt(eye, center, new Vector3(0.0f, 1.0f, 0.0f));
    }

    /// <summary>
    /// �J������Z�������𓾂܂��B
    /// </summary>
    /// <returns>Z������</returns>
    public Vector3 GetZAxis()
    {
        return new Vector3(pose.M31, pose.M32, pose.M33);
    }

    /// <summary>
    /// �J������Y�������𓾂܂��B
    /// </summary>
    /// <returns>Y������</returns>
    public Vector3 GetYAxis()
    {
        return new Vector3(pose.M21, pose.M22, pose.M23);
    }

    /// <summary>
    /// �J�����̈ʒu�Ǝp�����X�V���܂��B
    /// </summary>
    public void Update()
    {
        if (! needUpdate)
            return;

        //�J���� Z����]�Ŏp�������X�V
        pose = Matrix.RotationZ(rotZD) * pose;

        //�ܓx�o�x�̍����ړ�
        Vector3 localD = Vector3.TransformCoordinate(dirD, pose);
        if (localD.X != 0.0f || localD.Y != 0.0f || localD.Z != 0.0f)
        {
            //�J�����ʒu���X�V
            Vector3 zAxis = GetZAxis();
            Vector3 rotAxis = Vector3.Cross(localD, zAxis);
            Quaternion q = Quaternion.RotationAxis(rotAxis, angleU * dirD.Length());
            Matrix rotation = Matrix.RotationQuaternion(q);
            localP = Vector3.TransformCoordinate(localP, rotation);

            //�J�����p�����X�V
            Vector3 z = Vector3.Normalize(-localP);
            Vector3 y = GetYAxis();
            Vector3 x = Vector3.Normalize(Vector3.Cross(y, z));
            y = Vector3.Normalize(Vector3.Cross(z, x));
            {
                Matrix m = Matrix.Identity;
                m.M11 = x.X;
                m.M12 = x.Y;
                m.M13 = x.Z;
                m.M21 = y.X;
                m.M22 = y.Y;
                m.M23 = y.Z;
                m.M31 = z.X;
                m.M32 = z.Y;
                m.M33 = z.Z;
                pose = m;
            }
        }

        //���s�I�t�Z�b�g���X�V
        if (zD != 0.0f && localP.Length() - zD > 0)
        {
            Vector3 z = Vector3.Normalize(-localP);
            localP += zD * z;
        }

        //view�s��X�V
        Vector3 worldP = localP + center;
        {
            Matrix m = pose;
            m.M41 = worldP.X;
            m.M42 = worldP.Y;
            m.M43 = worldP.Z;
            m.M44 = 1.0f;
            view = Matrix.Invert(m) * Matrix.Translation(-translation);
        }

        //���������Z�b�g
        ResetDefValue();
        needUpdate = false;
    }

    /// <summary>
    /// view�s����擾���܂��B
    /// </summary>
    public Matrix GetViewMatrix()
    {
        return view;
    }

    /// <summary>
    /// ��]���S��ݒ肵�܂��B
    /// </summary>
    /// <param name="center">��]���S</param>
    public void SetCenter(Vector3 center)
    {
        this.center = center;
        needUpdate = true;
    }

    /// <summary>
    /// view���W��̈ʒu��ݒ肵�܂��B
    /// </summary>
    /// <param name="translation">view���W��̈ʒu</param>
    public void SetTranslation(Vector3 translation)
    {
        this.translation = translation;
        needUpdate = true;
    }

    /// <summary>
    /// view���W��ňړ����܂��B
    /// </summary>
    /// <param name="dx">X���ړ�����</param>
    /// <param name="dy">Y���ړ�����</param>
    public void MoveView(float dx, float dy)
    {
        this.translation.X += dx;
        this.translation.Y += dy;
        needUpdate = true;
    }

    /// <summary>
    /// ���������Z�b�g���܂��B
    /// </summary>
    protected void ResetDefValue()
    {
        dirD = Vector3.Empty;
        zD = 0.0f;
        rotZD = 0.0f;
    }

    private CameraMotion motion = null;

    /// <summary>
    /// �J�������[�V����
    /// </summary>
    public CameraMotion Motion
    {
        get { return motion; }
    }

    /// <summary>
    /// �J�������[�V������ݒ肵�܂��B
    /// </summary>
    /// <param name="frame_index">�t���[���ԍ�</param>
    /// <param name="eye">�����_</param>
    /// <param name="center">�J�����̈ʒu</param>
    public void SetMotion(int frame_index, Vector3 eye, Vector3 center)
    {
        motion.Add(frame_index, eye, center);
    }

    /// <summary>
    /// �J�������[�V������ݒ肵�܂��B
    /// </summary>
    /// <param name="frame_index">�t���[���ԍ�</param>
    /// <param name="eye">�����_</param>
    /// <param name="center">�J�����̈ʒu</param>
    /// <param name="interp_length">��Ԃ���t���[������</param>
    public void SetMotion(int frame_index, Vector3 eye, Vector3 center, int interp_length)
    {
        motion.Add(frame_index, eye, center, interp_length);
    }

    /// <summary>
    /// �J�������[�V������ݒ肵�܂��B
    /// </summary>
    /// <param name="frame_index">�t���[���ԍ�</param>
    /// <param name="eyex">�����_��X���W</param>
    /// <param name="eyey">�����_��Y���W</param>
    /// <param name="eyez">�����_��Z���W</param>
    /// <param name="centerx">�J�����̈ʒu��X���W</param>
    /// <param name="centery">�J�����̈ʒu��Y���W</param>
    /// <param name="centerz">�J�����̈ʒu��Z���W</param>
    public void SetMotion(int frame_index, float eyex, float eyey, float eyez, float centerx, float centery, float centerz)
    {
        motion.Add(frame_index, new Vector3(eyex, eyey, eyez), new Vector3(centerx, centery, centerz));
    }

    /// <summary>
    /// �J�������[�V������ݒ肵�܂��B
    /// </summary>
    /// <param name="frame_index">�t���[���ԍ�</param>
    /// <param name="eyex">�����_��X���W</param>
    /// <param name="eyey">�����_��Y���W</param>
    /// <param name="eyez">�����_��Z���W</param>
    /// <param name="centerx">�J�����̈ʒu��X���W</param>
    /// <param name="centery">�J�����̈ʒu��Y���W</param>
    /// <param name="centerz">�J�����̈ʒu��Z���W</param>
    /// <param name="interp_length">��Ԃ���t���[������</param>
    public void SetMotion(int frame_index, float eyex, float eyey, float eyez, float centerx, float centery, float centerz, int interp_length)
    {
        motion.Add(frame_index, new Vector3(eyex, eyey, eyez), new Vector3(centerx, centery, centerz), interp_length);
    }

    /// <summary>
    /// �w�胂�[�V�����t���[���ɐi�݂܂��B
    /// </summary>
    public void SetFrameIndex(int frame_index)
    {
        if (motion.Count != 0)
        {
            motion.SetFrameIndex(frame_index);
        }
    }
}
}