using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace mx
{
	public class MeshBuilder
	{
		private List<Vector3> m_vertices = null;
		private List<int> m_indices = null;
		private List<Color> m_colors = null;
		private List<Vector2> m_uvs = null;

		public Mesh mesh { get; private set; }
		public int vertexCount { get { return (m_vertices != null ? m_vertices.Count : 0); } }
		
		public MeshBuilder()
		{
		}

		public void AddVertex(Vector3 v, Color? c = null, Vector2? uv = null)
		{
			if (m_vertices == null)
			{
				m_vertices = new List<Vector3>();
			}
			m_vertices.Add(v);
			
			if (c.HasValue)
			{
				if (m_colors == null)
				{
					m_colors = new List<Color>();
				}
				m_colors.Add(c.Value);
			}
			else if (m_colors != null)
			{
				m_colors.Add(Color.white);
				Debug.LogWarning(string.Format("MeshBuilder -- Missing Color for vertex [{0}:{1}] when other colors have been assigned.",
					m_vertices.Count - 1, v));
			}

			if (uv.HasValue)
			{
				if (m_uvs == null)
				{
					m_uvs = new List<Vector2>();
				}
				m_uvs.Add(uv.Value);
			}
			else if (m_uvs != null)
			{
				m_uvs.Add(Vector2.zero);
				Debug.LogWarning(string.Format("MeshBuilder -- Missing uv for vertex [{0}:{1}] when other uvs have been assigned.",
					m_vertices.Count - 1, v));
			}
		}

		public void AddIndex(int i)
		{
			if (m_indices == null)
			{
				m_indices = new List<int>();
			}
			m_indices.Add(i);
		}

		public void Build(HideFlags hideFlags, MeshTopology topology)
		{
			Mesh m = new Mesh();
			if (m_vertices != null)
			{
				m.vertices = m_vertices.ToArray();
			}

			if (m_colors != null)
			{
				m.colors = m_colors.ToArray();
			}

			if (m_indices != null)
			{
				m.SetIndices(m_indices.ToArray(), topology, 0);
			}

			if (m_uvs != null)
			{
				m.uv = m_uvs.ToArray();
			}

			m.RecalculateBounds();
			m.hideFlags = hideFlags;
            this.mesh = m;
		}

		public void Append(MeshBuilder other)
		{
			if (other == null)
			{
				return;
			}

			int indexOffset = (m_vertices != null ? m_vertices.Count : 0);

			if (other.m_vertices != null)
			{
				if (m_vertices == null)
				{
					m_vertices = new List<Vector3>();
				}
				m_vertices.AddRange(other.m_vertices);
			}

			if (other.m_indices != null)
			{
				if (m_indices == null)
				{
					m_indices = new List<int>();
				}

				m_indices.AddRange(other.m_indices);

				for (int i = indexOffset; i < m_indices.Count; ++i)
				{
					m_indices[i] += indexOffset;
				}
			}

			if (other.m_colors != null)
			{
				if (m_colors == null)
				{
					m_colors = new List<Color>();
				}
				m_colors.AddRange(other.m_colors);
			}

			if (other.m_uvs != null)
			{
				if (m_uvs == null)
				{
					m_uvs = new List<Vector2>();
				}
				m_uvs.AddRange(other.m_uvs);
			}
		}

		public void AddSubmesh(MeshBuilder submesh)
		{

		}
	}
}