/* 
 * Public domain software, no restrictions. 
 * Released by rickd3ckard: https://github.com/rickd3ckard 
 * See: https://unlicense.org/
 */

using CDP.Utils;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CDP.Objects
{
    public class Node
    {
        public Node() { }

        [JsonIgnore]
        public DOM? DOM { get; set; }

        [JsonPropertyName("nodeId")]
        public int? NodeId { get; set; }

        [JsonPropertyName("parentId")]
        public int? ParentId { get; set; }

        [JsonPropertyName("backendNodeId")]
        public int? BackendNodeId { get; set; }

        [JsonPropertyName("nodeType")]
        public int? NodeType { get; set; }

        [JsonPropertyName("nodeName")]
        public string? NodeName { get; set; }

        [JsonPropertyName("localName")]
        public string? LocalName { get; set; }

        [JsonPropertyName("nodeValue")]
        public string? NodeValue { get; set; }

        [JsonPropertyName("childNodeCount")]
        public int? ChildNodeCount { get; set; }

        [JsonPropertyName("children")]
        public Node[]? Children { get; set; }

        [JsonPropertyName("attributes")]
        public string[]? Attributes { get; set; }

        [JsonPropertyName("documentURL")]
        public string? DocumentURL { get; set; }

        [JsonPropertyName("baseURL")]
        public string? BaseURL { get; set; }

        [JsonPropertyName("publicId")]
        public string? PublicId { get; set; }

        [JsonPropertyName("systemId")]
        public string? SystemId { get; set; }

        [JsonPropertyName("internalSubset")]
        public string? InternalSubset { get; set; }

        [JsonPropertyName("xmlVersion")]
        public string? XmlVersion { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("value")]
        public string? Value { get; set; }

        [JsonPropertyName("pseudoType")]
        public string? PseudoType { get; set; }

        [JsonPropertyName("pseudoIdentifier")]
        public string? PseudoIdentifier { get; set; }

        [JsonPropertyName("shadowRootType")]
        public string? ShadowRootType { get; set; }

        [JsonPropertyName("frameId")]
        public string? FrameId { get; set; }

        [JsonPropertyName("shadowRoots")]
        public Node[]? ShadowRoots { get; set; }

        [JsonPropertyName("isSVG")]
        public bool? IsSvg { get; set; }

        [JsonPropertyName("compatibilityMode")]
        public string? CompatibilityMode { get; set; }

        [JsonPropertyName("isScrollable")]
        public bool? IsScrollable { get; set; }

        //public BackendNode AssignedSlot { get; set; }
        //public Node ContentDocument { get; set; }   
        //public Node TemplateContent { get; set; }
        //public Node[] PseudoElements { get; set; }
        //public Node ImportedDocument { get; set; }
        //public BackendNode[] DistributedNodes { get; set; }

        public string? GetAttributeValue(string Attribute)
        {
            if (this.Attributes == null) { return null; }
            int attributeIndex = Array.IndexOf(this.Attributes, Attribute);
            if (attributeIndex == -1) { return null; }
            if (attributeIndex + 2 > this.Attributes.Length) { return null; }

            return this.Attributes[attributeIndex + 1];
        }

        public override string ToString()
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = true;
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

            return JsonSerializer.Serialize(this, options); 
        }

        #region Facade Methodes 
        public async Task Click(MouseButtonEnum MouseButton, bool ScrollIntoViewIfNeed = false) // could maybe check if center is inside view port first and cancel if not
        {
            if (this.DOM == null) { throw new NullReferenceException(); }
            if (this.NodeId == 0 || this.NodeId == null) { throw new NullReferenceException(); }

            if (ScrollIntoViewIfNeed) { await this.DOM.ScrollIntoViewIfNeeded(this.NodeId.Value); }
            
            BoxModel box = await this.DOM.GetBoxModel(this.NodeId.Value);         
            await this.DOM.DispatchMouseEvent(box.Center, MouseButton);
        }

        public async Task ScrollIntoView()
        {
            if (this.DOM == null) { throw new NullReferenceException(); }
            if (this.NodeId == 0 || this.NodeId == null) { throw new NullReferenceException(); }

            await this.DOM.ScrollIntoViewIfNeeded(this.NodeId.Value);
        }

        public string? GetText()
        {
            if (this.Children == null || this.Children.Count() <= 0) { return null; }
            Node? textNode = this.Children.FirstOrDefault(n => n.NodeName == "#text");
            if (textNode == null) { return null; }
            return textNode.NodeValue;
        }
        #endregion
    }
}
