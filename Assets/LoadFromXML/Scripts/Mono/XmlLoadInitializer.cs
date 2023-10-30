using System.Xml;
using Data;
using LoadFromXML.Scripts.ComponentAndTags;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace LoadFromXML.Scripts.Mono
{
    public class XmlLoadInitializer : MonoBehaviour
    {
        public string osmPath;

        private NativeArray<NodeData> _nodeDataNativeArray;
        private NativeArray<WayData> _wayDataNativeArray;
        private NativeArray<LaneletData> _laneletDataNativeArray;
        private NativeList<int> _nodeListForWay; // Node ID Reference List for Ways

        private XmlNode _xmlOsm;
        private XmlNodeList _xmlNodesList;
        private XmlNodeList _xmlWaysList;
        private XmlNodeList _xmlLaneletsList;
        
        private void OnDisable()
        {
            _nodeDataNativeArray.Dispose();
            _wayDataNativeArray.Dispose();
            _laneletDataNativeArray.Dispose();
            _nodeListForWay.Dispose();
        }

        [ContextMenu("Load XML")]
        private void InitializeXMLLoad()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(osmPath);
            _xmlOsm = xmlDocument.SelectSingleNode("osm");
            
            ReadNodes();
            ReadWays();
            ReadLanelets();
            SpawnEntity();
        }
        
        // Nodes
        private void ReadNodes()
        {
            _xmlNodesList = _xmlOsm.SelectNodes("node");
            _nodeDataNativeArray = new NativeArray<NodeData>(_xmlNodesList!.Count, Allocator.Persistent);

            for (var i = 0; i < _xmlNodesList.Count; i++)
            {
                var xmlNode = _xmlNodesList[i];
                _nodeDataNativeArray[i] = new NodeData
                {
                    ID = int.Parse(xmlNode.Attributes!["id"].Value),
                    Position = GetNodePositionValue(xmlNode.SelectNodes("tag"))
                };
            }
        }

        private float3 GetNodePositionValue(XmlNodeList xmlPiece)
        {
            float3 position = new float3();
            foreach (XmlNode xmlTag in xmlPiece)
            {
                switch (xmlTag.Attributes!["k"].Value)
                {
                    case "ele":
                        position.y = float.Parse(xmlTag.Attributes["v"].Value);
                        break;
                    case "local_x":
                        position.x = float.Parse(xmlTag.Attributes["v"].Value);
                        break;
                    case "local_y":
                        position.z = float.Parse(xmlTag.Attributes["v"].Value);
                        break;
                }
            }

            return position;
        }
        
        // Ways
        private void ReadWays()
        {
            _xmlWaysList = _xmlOsm.SelectNodes("way");
            _wayDataNativeArray = new NativeArray<WayData>(_xmlWaysList!.Count, Allocator.Persistent);
            _nodeListForWay = new NativeList<int>(_xmlWaysList.Count * 12, Allocator.Persistent);
            
            var sliceCounter = 0;
            for (var i = 0; i < _xmlWaysList.Count; i++)
            {
                var xmlWay = _xmlWaysList[i];
                GetWayNodes(xmlWay.SelectNodes("nd"));
                _wayDataNativeArray[i] = new WayData
                {
                    ID = int.Parse(xmlWay.Attributes!["id"].Value),
                    Type = GetWayType(xmlWay.SelectNodes("tag")),
                    SliceStartId = sliceCounter,
                    SliceEndId = _nodeListForWay.Length - 1
                };
                sliceCounter = _nodeListForWay.Length;
            }
        }

        private void GetWayNodes(XmlNodeList xmlPiece)
        {
            for (var i = 0; i < xmlPiece.Count; i++)
            {
                var nodeID = int.Parse(xmlPiece[i].Attributes!["ref"].Value);
                _nodeListForWay.Add(nodeID);
            }
        }

        private WayDataTypes GetWayType(XmlNodeList xmlPiece)
        {
            WayDataTypes wayType = WayDataTypes.None;
            foreach (XmlNode xmlTag in xmlPiece)
            {
                switch (xmlTag.Attributes!["k"].Value)
                {
                    case "subtype":
                        switch (xmlTag.Attributes["v"].Value)
                        {
                            case "solid":
                                wayType |= WayDataTypes.Solid;
                                break;
                            case "dashed":
                                wayType |= WayDataTypes.Dashed;
                                break;
                            case "bidirectional":
                                wayType |= WayDataTypes.Bidirectional;
                                break;
                        }
                        break;
                    case "bidirectional":
                        switch (xmlTag.Attributes["v"].Value)
                        {
                            case "true":
                                wayType |= WayDataTypes.Bidirectional;
                                break;
                            case "false":
                                break;
                            default:
                                Debug.Log("Unsupported Way bidirectional value: " + xmlTag.Attributes["v"].Value);
                                break;
                        }
                        break;
                }
            }

            return wayType;
        }
        
        // Lanelets
        private void ReadLanelets()
        {
            _xmlLaneletsList = _xmlOsm.SelectNodes("relation");
            _laneletDataNativeArray = new NativeArray<LaneletData>(_xmlLaneletsList!.Count, Allocator.Persistent);

            for (var i = 0; i < _xmlLaneletsList.Count; i++)
            {
                var xmlLanelet = _xmlLaneletsList[i];
                var laneletData = new LaneletData
                {
                    ID = int.Parse(xmlLanelet.Attributes!["id"].Value),
                    Type = GetLaneletType(xmlLanelet.SelectNodes("tag")),
                    SpeedLimit = GetLaneletSpeedLimit(xmlLanelet.SelectNodes("tag"))
                };
                GetLaneletMembers(xmlLanelet.SelectNodes("member"), ref laneletData);
                _laneletDataNativeArray[i] = laneletData;
            }
        }
        
        private LaneletDataTypes GetLaneletType(XmlNodeList xmlPiece)
        {
            var laneletType = LaneletDataTypes.None;
            
            foreach (XmlNode xmlTag in xmlPiece)
            {
                switch (xmlTag.Attributes!["k"].Value)
                {
                    case "subtype":
                        switch (xmlTag.Attributes["v"].Value)
                        {
                            case "bicycle_lane":
                                laneletType |= LaneletDataTypes.TypeBicycle;
                                break;
                            case "crosswalk":
                                laneletType |= LaneletDataTypes.TypeCrosswalk;
                                break;
                            default:
                                laneletType |= LaneletDataTypes.TypeRoad;
                                break;
                        }
                        break;
                    case "turn_direction":
                        switch (xmlTag.Attributes["v"].Value)
                        {
                            case "straight":
                                laneletType |= LaneletDataTypes.TurnStraight;
                                break;
                            case "left":
                                laneletType |= LaneletDataTypes.TurnLeft;
                                break;
                            case "right":
                                laneletType |= LaneletDataTypes.TurnRight;
                                break;
                        }
                        break;
                    case "reverse_line":
                        switch (xmlTag.Attributes["v"].Value)
                        {
                            case "left":
                                laneletType |= LaneletDataTypes.ReverseLeft;
                                break;
                            case "right":
                                laneletType |= LaneletDataTypes.ReverseRight;
                                break;
                            default:
                                laneletType |= LaneletDataTypes.ReverseNone;
                                break;
                        }
                        break;
                    case "speed_limit":
                        laneletType |= LaneletDataTypes.SpeedLimitTrue;
                        break;
                }
            }
            
            return laneletType;
        }

        private int GetLaneletSpeedLimit(XmlNodeList xmlPiece)
        {
            int speedLimit = 0;
            foreach (XmlNode xmlTag in xmlPiece)
            {
                if (xmlTag.Attributes!["k"].Value == "speed_limit")
                {
                    speedLimit = int.Parse(xmlTag.Attributes["v"].Value);
                }
            }
            return speedLimit;
        }
        
        private void GetLaneletMembers(XmlNodeList xmlPiece, ref LaneletData laneletData)
        {
            foreach (XmlNode member in xmlPiece)
            {
                if (member.Attributes!["type"].Value == "way")
                {
                    int laneletMember = int.Parse(member.Attributes["ref"].Value);
                    switch (member.Attributes["role"].Value)
                    {
                        case "left":
                            laneletData.LeftWayId = laneletMember;
                            break;
                        case "right":
                            laneletData.RightWayId = laneletMember;
                            break;
                        case "centerline":
                            laneletData.MiddleWayId = laneletMember;
                            break;
                    }
                }
            }
        }
        
        // Spawn Entity
        private void SpawnEntity()
        {
            var loadComponent = new LoadComponent
            {
                NodeDataNativeArray = _nodeDataNativeArray,
                WayDataNativeArray = _wayDataNativeArray,
                LaneletDataNativeArray = _laneletDataNativeArray,
                NodeListForWayNativeList = _nodeListForWay
            };
            
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, loadComponent);
        }
    }
}