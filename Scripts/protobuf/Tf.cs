// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: tf.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Tf {

  /// <summary>Holder for reflection information generated from tf.proto</summary>
  public static partial class TfReflection {

    #region Descriptor
    /// <summary>File descriptor for tf.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static TfReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Cgh0Zi5wcm90bxICdGYaCXN0ZC5wcm90bxoOZ2VvbWV0cnkucHJvdG8iNwoO",
            "VGZGcmFtZVJlcXVlc3QSDwoHZnJhbWVJZBgBIAEoCRIUCgxjaGlsZEZyYW1l",
            "SWQYAiABKAkirgEKB1RmRnJhbWUSGwoGaGVhZGVyGAEgASgLMgsuc3RkLkhl",
            "YWRlchIPCgdmcmFtZUlkGAIgASgJEhQKDGNoaWxkRnJhbWVJZBgDIAEoCRIm",
            "Cgt0cmFuc2xhdGlvbhgEIAEoCzIRLmdlb21ldHJ5LlZlY3RvcjMSJgoIcm90",
            "YXRpb24YBSABKAsyFC5nZW9tZXRyeS5RdWF0ZXJuaW9uEg8KB2FkZHJlc3MY",
            "BiABKAkiKgoLVGZGcmFtZUxpc3QSGwoGZnJhbWVzGAEgAygLMgsudGYuVGZG",
            "cmFtZTL3AQoCVGYSLQoMR2V0QWxsRnJhbWVzEgouc3RkLkVtcHR5Gg8udGYu",
            "VGZGcmFtZUxpc3QiABItCghHZXRGcmFtZRISLnRmLlRmRnJhbWVSZXF1ZXN0",
            "GgsudGYuVGZGcmFtZSIAEjIKD1N0cmVhbUFsbEZyYW1lcxIKLnN0ZC5FbXB0",
            "eRoPLnRmLlRmRnJhbWVMaXN0IgAwARIyCgtTdHJlYW1GcmFtZRISLnRmLlRm",
            "RnJhbWVSZXF1ZXN0GgsudGYuVGZGcmFtZSIAMAESKwoMUHVibGlzaEZyYW1l",
            "EgsudGYuVGZGcmFtZRoKLnN0ZC5FbXB0eSIAKAFiBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Std.StdReflection.Descriptor, global::Geometry.GeometryReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Tf.TfFrameRequest), global::Tf.TfFrameRequest.Parser, new[]{ "FrameId", "ChildFrameId" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Tf.TfFrame), global::Tf.TfFrame.Parser, new[]{ "Header", "FrameId", "ChildFrameId", "Translation", "Rotation", "Address" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Tf.TfFrameList), global::Tf.TfFrameList.Parser, new[]{ "Frames" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class TfFrameRequest : pb::IMessage<TfFrameRequest> {
    private static readonly pb::MessageParser<TfFrameRequest> _parser = new pb::MessageParser<TfFrameRequest>(() => new TfFrameRequest());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<TfFrameRequest> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Tf.TfReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public TfFrameRequest() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public TfFrameRequest(TfFrameRequest other) : this() {
      frameId_ = other.frameId_;
      childFrameId_ = other.childFrameId_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public TfFrameRequest Clone() {
      return new TfFrameRequest(this);
    }

    /// <summary>Field number for the "frameId" field.</summary>
    public const int FrameIdFieldNumber = 1;
    private string frameId_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string FrameId {
      get { return frameId_; }
      set {
        frameId_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "childFrameId" field.</summary>
    public const int ChildFrameIdFieldNumber = 2;
    private string childFrameId_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string ChildFrameId {
      get { return childFrameId_; }
      set {
        childFrameId_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as TfFrameRequest);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(TfFrameRequest other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (FrameId != other.FrameId) return false;
      if (ChildFrameId != other.ChildFrameId) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (FrameId.Length != 0) hash ^= FrameId.GetHashCode();
      if (ChildFrameId.Length != 0) hash ^= ChildFrameId.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (FrameId.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(FrameId);
      }
      if (ChildFrameId.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(ChildFrameId);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (FrameId.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(FrameId);
      }
      if (ChildFrameId.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(ChildFrameId);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(TfFrameRequest other) {
      if (other == null) {
        return;
      }
      if (other.FrameId.Length != 0) {
        FrameId = other.FrameId;
      }
      if (other.ChildFrameId.Length != 0) {
        ChildFrameId = other.ChildFrameId;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            FrameId = input.ReadString();
            break;
          }
          case 18: {
            ChildFrameId = input.ReadString();
            break;
          }
        }
      }
    }

  }

  public sealed partial class TfFrame : pb::IMessage<TfFrame> {
    private static readonly pb::MessageParser<TfFrame> _parser = new pb::MessageParser<TfFrame>(() => new TfFrame());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<TfFrame> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Tf.TfReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public TfFrame() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public TfFrame(TfFrame other) : this() {
      header_ = other.header_ != null ? other.header_.Clone() : null;
      frameId_ = other.frameId_;
      childFrameId_ = other.childFrameId_;
      translation_ = other.translation_ != null ? other.translation_.Clone() : null;
      rotation_ = other.rotation_ != null ? other.rotation_.Clone() : null;
      address_ = other.address_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public TfFrame Clone() {
      return new TfFrame(this);
    }

    /// <summary>Field number for the "header" field.</summary>
    public const int HeaderFieldNumber = 1;
    private global::Std.Header header_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Std.Header Header {
      get { return header_; }
      set {
        header_ = value;
      }
    }

    /// <summary>Field number for the "frameId" field.</summary>
    public const int FrameIdFieldNumber = 2;
    private string frameId_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string FrameId {
      get { return frameId_; }
      set {
        frameId_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "childFrameId" field.</summary>
    public const int ChildFrameIdFieldNumber = 3;
    private string childFrameId_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string ChildFrameId {
      get { return childFrameId_; }
      set {
        childFrameId_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "translation" field.</summary>
    public const int TranslationFieldNumber = 4;
    private global::Geometry.Vector3 translation_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Geometry.Vector3 Translation {
      get { return translation_; }
      set {
        translation_ = value;
      }
    }

    /// <summary>Field number for the "rotation" field.</summary>
    public const int RotationFieldNumber = 5;
    private global::Geometry.Quaternion rotation_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Geometry.Quaternion Rotation {
      get { return rotation_; }
      set {
        rotation_ = value;
      }
    }

    /// <summary>Field number for the "address" field.</summary>
    public const int AddressFieldNumber = 6;
    private string address_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Address {
      get { return address_; }
      set {
        address_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as TfFrame);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(TfFrame other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(Header, other.Header)) return false;
      if (FrameId != other.FrameId) return false;
      if (ChildFrameId != other.ChildFrameId) return false;
      if (!object.Equals(Translation, other.Translation)) return false;
      if (!object.Equals(Rotation, other.Rotation)) return false;
      if (Address != other.Address) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (header_ != null) hash ^= Header.GetHashCode();
      if (FrameId.Length != 0) hash ^= FrameId.GetHashCode();
      if (ChildFrameId.Length != 0) hash ^= ChildFrameId.GetHashCode();
      if (translation_ != null) hash ^= Translation.GetHashCode();
      if (rotation_ != null) hash ^= Rotation.GetHashCode();
      if (Address.Length != 0) hash ^= Address.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (header_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Header);
      }
      if (FrameId.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(FrameId);
      }
      if (ChildFrameId.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(ChildFrameId);
      }
      if (translation_ != null) {
        output.WriteRawTag(34);
        output.WriteMessage(Translation);
      }
      if (rotation_ != null) {
        output.WriteRawTag(42);
        output.WriteMessage(Rotation);
      }
      if (Address.Length != 0) {
        output.WriteRawTag(50);
        output.WriteString(Address);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (header_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Header);
      }
      if (FrameId.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(FrameId);
      }
      if (ChildFrameId.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(ChildFrameId);
      }
      if (translation_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Translation);
      }
      if (rotation_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Rotation);
      }
      if (Address.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Address);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(TfFrame other) {
      if (other == null) {
        return;
      }
      if (other.header_ != null) {
        if (header_ == null) {
          header_ = new global::Std.Header();
        }
        Header.MergeFrom(other.Header);
      }
      if (other.FrameId.Length != 0) {
        FrameId = other.FrameId;
      }
      if (other.ChildFrameId.Length != 0) {
        ChildFrameId = other.ChildFrameId;
      }
      if (other.translation_ != null) {
        if (translation_ == null) {
          translation_ = new global::Geometry.Vector3();
        }
        Translation.MergeFrom(other.Translation);
      }
      if (other.rotation_ != null) {
        if (rotation_ == null) {
          rotation_ = new global::Geometry.Quaternion();
        }
        Rotation.MergeFrom(other.Rotation);
      }
      if (other.Address.Length != 0) {
        Address = other.Address;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            if (header_ == null) {
              header_ = new global::Std.Header();
            }
            input.ReadMessage(header_);
            break;
          }
          case 18: {
            FrameId = input.ReadString();
            break;
          }
          case 26: {
            ChildFrameId = input.ReadString();
            break;
          }
          case 34: {
            if (translation_ == null) {
              translation_ = new global::Geometry.Vector3();
            }
            input.ReadMessage(translation_);
            break;
          }
          case 42: {
            if (rotation_ == null) {
              rotation_ = new global::Geometry.Quaternion();
            }
            input.ReadMessage(rotation_);
            break;
          }
          case 50: {
            Address = input.ReadString();
            break;
          }
        }
      }
    }

  }

  public sealed partial class TfFrameList : pb::IMessage<TfFrameList> {
    private static readonly pb::MessageParser<TfFrameList> _parser = new pb::MessageParser<TfFrameList>(() => new TfFrameList());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<TfFrameList> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Tf.TfReflection.Descriptor.MessageTypes[2]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public TfFrameList() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public TfFrameList(TfFrameList other) : this() {
      frames_ = other.frames_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public TfFrameList Clone() {
      return new TfFrameList(this);
    }

    /// <summary>Field number for the "frames" field.</summary>
    public const int FramesFieldNumber = 1;
    private static readonly pb::FieldCodec<global::Tf.TfFrame> _repeated_frames_codec
        = pb::FieldCodec.ForMessage(10, global::Tf.TfFrame.Parser);
    private readonly pbc::RepeatedField<global::Tf.TfFrame> frames_ = new pbc::RepeatedField<global::Tf.TfFrame>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Tf.TfFrame> Frames {
      get { return frames_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as TfFrameList);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(TfFrameList other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if(!frames_.Equals(other.frames_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      hash ^= frames_.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      frames_.WriteTo(output, _repeated_frames_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += frames_.CalculateSize(_repeated_frames_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(TfFrameList other) {
      if (other == null) {
        return;
      }
      frames_.Add(other.frames_);
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            frames_.AddEntriesFrom(input, _repeated_frames_codec);
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
