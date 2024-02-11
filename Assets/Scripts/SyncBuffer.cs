using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class SyncBuffer : MonoBehaviour
{
    [Range(0.05f, 0.5f)]
    public float TargetLatency = 0.075f;

    [NonSerialized]
    public float ErrorCorrectionSpeed = 10f;

    [NonSerialized]
    public float TimeCorrectionSpeed = 3f;

    [NonSerialized]
    public bool DisableExtrapolation;

    protected float _playbackTime;

    protected float _timeDrift;

    protected List<SyncBuffer.Keyframe> _keyframes = new List<SyncBuffer.Keyframe>();

    [NonSerialized]
    public Vector3 ExtrapolationPositionDrift;

    [NonSerialized]
    public Quaternion ExtrapolationRotationDrift;

    public virtual bool IsExtrapolating
	{
		get
		{
			return this._keyframes.Count == 1;
		}
	}

	public virtual Vector3 Position
	{
		get
		{
			return this.PositionNoErrorCorrection + this.ExtrapolationPositionDrift;
		}
	}

	public virtual Vector3 PositionNoErrorCorrection
	{
		get
		{
			if (!this.HasKeyframes)
			{
				Debug.LogWarning("Trying to access position in an empty buffer. Zero vector returned.");
				return Vector3.zero;
			}
			if (this.DisableExtrapolation && this.IsExtrapolating)
			{
				return this._keyframes[0].Position;
			}
			return this._keyframes[0].Position + this._keyframes[0].Velocity * this._playbackTime + 0.5f * this._keyframes[0].Acceleration * this._playbackTime * this._playbackTime;
		}
	}

	public virtual Vector3 Velocity
	{
		get
		{
			if (!this.HasKeyframes)
			{
				Debug.LogWarning("Trying to access velocity in an empty buffer. Zero vector returned.");
				return Vector3.zero;
			}
			return this._keyframes[0].Velocity + this._keyframes[0].Acceleration * this._playbackTime;
		}
	}

	public virtual Quaternion Rotation
	{
		get
		{
			return this.RotationNoErrorCorrection * this.ExtrapolationRotationDrift;
		}
	}

	public virtual Quaternion RotationNoErrorCorrection
	{
		get
		{
			if (!this.HasKeyframes)
			{
				Debug.LogWarning("Trying to access rotation in an empty buffer. Zero rotation returned.");
				return Quaternion.identity;
			}
			if (this.DisableExtrapolation && this.IsExtrapolating)
			{
				return this._keyframes[0].Rotation;
			}
			return this._keyframes[0].Rotation * Quaternion.Euler(this._keyframes[0].AngularVelocity * this._playbackTime + 0.5f * this._keyframes[0].AngularAcceleration * this._playbackTime * this._playbackTime);
		}
	}

	public virtual Vector3 AngularVelocity
	{
		get
		{
			if (!this.HasKeyframes)
			{
				Debug.LogWarning("Trying to access angular velocity in an empty buffer. Zero vector returned.");
				return Vector3.zero;
			}
			return this._keyframes[0].AngularVelocity + this._keyframes[0].AngularAcceleration * this._playbackTime;
		}
	}


	public virtual bool HasKeyframes
	{
		get
		{
			return this._keyframes.Count != 0;
		}
	}

	public virtual SyncBuffer.Keyframe LastReceivedKeyframe
	{
		get
		{
			if (!this.HasKeyframes)
			{
				Debug.LogWarning("Trying to access LastReceivedKeyframe in an empty buffer. Blank keyframe returned.");
				return default(SyncBuffer.Keyframe);
			}
			return this._keyframes[this._keyframes.Count - 1];
		}
	}

	public virtual SyncBuffer.Keyframe NextKeyframe
	{
		get
		{
			if (this._keyframes.Count < 2)
			{
				Debug.LogWarning("Trying to access NextKeyframe in a buffer that is empty or currently extrapolating. Blank keyframe returned.");
				return default(SyncBuffer.Keyframe);
			}
			return this._keyframes[1];
		}
	}

	public virtual SyncBuffer.Keyframe CurrentKeyframe
	{
		get
		{
			if (!this.HasKeyframes)
			{
				Debug.LogWarning("Trying to access CurrentKeyframe in an empty buffer. Blank keyframe returned.");
				return default(SyncBuffer.Keyframe);
			}
			return this._keyframes[0];
		}
	}

	public virtual void AddKeyframe(float interpolationTime, Vector3 position, [Optional] Vector3 acceleration, [Optional] Vector3 angularAcceleration, [Optional] Quaternion rotation, Vector3? velocity = null, Vector3? angularVelocity = null)
	{
		if (this._keyframes.Count < 1)
		{
			interpolationTime = Mathf.Max(this.TargetLatency, 0.01f);
		}
		float num = interpolationTime - this._playbackTime;
		for (int i = 1; i < this._keyframes.Count; i++)
		{
			num += this._keyframes[i].InterpolationTime;
		}
		this._timeDrift = this.TargetLatency - num;
		SyncBuffer.Keyframe keyframe;
		if (this._keyframes.Count < 1)
		{
			keyframe = new SyncBuffer.Keyframe
			{
				Position = position,
				Velocity = Vector3.zero,
				Acceleration = Vector3.zero,
				Rotation = rotation,
				AngularVelocity = Vector3.zero,
				AngularAcceleration = Vector3.zero
			};
			SyncBuffer.Keyframe item = keyframe;
			this._keyframes.Add(item);
		}
		Vector3 position2 = this.Position;
		Quaternion rotation2 = this.Rotation;
		SyncBuffer.Keyframe lastReceivedKeyframe = this.LastReceivedKeyframe;
		Vector3 vector = (interpolationTime <= 0f) ? Vector3.zero : ((position - lastReceivedKeyframe.Position) / interpolationTime);
		Quaternion rotationDifference = this.GetRotationDifference(lastReceivedKeyframe.Rotation, rotation);
		Vector3 vector2 = (interpolationTime <= 0f) ? Vector3.zero : (this.FormatEulerRotation180(rotationDifference.eulerAngles) / interpolationTime);
		keyframe = new SyncBuffer.Keyframe
		{
			InterpolationTime = interpolationTime,
			Position = position,
			Velocity = ((velocity == null) ? vector : velocity.Value),
			Acceleration = acceleration,
			Rotation = rotation,
			AngularVelocity = ((angularVelocity == null) ? vector2 : angularVelocity.Value),
			AngularAcceleration = angularAcceleration
		};
		SyncBuffer.Keyframe item2 = keyframe;
		this._keyframes.Add(item2);
		lastReceivedKeyframe.Velocity = vector;
		lastReceivedKeyframe.AngularVelocity = vector2;
		lastReceivedKeyframe.Acceleration = Vector3.zero;
		lastReceivedKeyframe.AngularAcceleration = Vector3.zero;
		this._keyframes[this._keyframes.Count - 2] = lastReceivedKeyframe;
		this.UpdatePlayback(0f);
		Vector3 positionNoErrorCorrection = this.PositionNoErrorCorrection;
		Quaternion rotationNoErrorCorrection = this.RotationNoErrorCorrection;
		this.ExtrapolationPositionDrift = position2 - positionNoErrorCorrection;
		this.ExtrapolationRotationDrift = this.GetRotationDifference(rotationNoErrorCorrection, rotation2);
	}

	public virtual void UpdatePlayback(float deltaTime)
	{
		if (this._keyframes.Count < 1)
		{
			Debug.LogWarning("Trying to update playback in an empty buffer.");
			return;
		}
		if (deltaTime > 0f)
		{
			this._playbackTime += deltaTime;
			float num = -Mathf.Lerp(0f, this._timeDrift, this.TimeCorrectionSpeed * deltaTime);
			this._playbackTime += num;
			this._timeDrift += num;
			this.ExtrapolationPositionDrift = Vector3.Lerp(this.ExtrapolationPositionDrift, Vector3.zero, this.ErrorCorrectionSpeed * deltaTime);
			this.ExtrapolationRotationDrift = Quaternion.Lerp(this.ExtrapolationRotationDrift, Quaternion.identity, this.ErrorCorrectionSpeed * deltaTime);
		}
		while (this._keyframes.Count > 1 && this._playbackTime >= this._keyframes[1].InterpolationTime)
		{
			if (this._keyframes[1].InterpolationTime == 0f)
			{
				this.ExtrapolationPositionDrift = Vector3.zero;
				this.ExtrapolationRotationDrift = Quaternion.identity;
			}
			this._playbackTime -= this._keyframes[1].InterpolationTime;
			this._keyframes.RemoveAt(0);
		}
	}

	protected Quaternion GetRotationDifference(Quaternion fromRotation, Quaternion toRotation)
	{
		return Quaternion.Inverse(fromRotation) * toRotation;
	}

	protected Vector3 FormatEulerRotation180(Vector3 eulerRotation)
	{
		while (eulerRotation.x > 180f)
		{
			eulerRotation.x -= 360f;
		}
		while (eulerRotation.y > 180f)
		{
			eulerRotation.y -= 360f;
		}
		while (eulerRotation.z > 180f)
		{
			eulerRotation.z -= 360f;
		}
		while (eulerRotation.x <= -180f)
		{
			eulerRotation.x += 360f;
		}
		while (eulerRotation.y <= -180f)
		{
			eulerRotation.y += 360f;
		}
		while (eulerRotation.z <= -180f)
		{
			eulerRotation.z += 360f;
		}
		return eulerRotation;
	}

	public struct Keyframe
	{
		public float InterpolationTime;

		public Vector3 Position;

		public Vector3 Velocity;

		public Vector3 Acceleration;

		public Quaternion Rotation;

		public Vector3 AngularVelocity;

		public Vector3 AngularAcceleration;
	}
}
