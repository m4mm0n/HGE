using System;
using System.Collections.Generic;

namespace HGE.Graphics
{
    public static class Graphics2D3DGL
    {
        public enum RENDERFLAGS
        {
            RENDER_WIRE = 0x01,
            RENDER_FLAT = 0x02,
            RENDER_TEXTURED = 0x04,
            RENDER_CULL_CW = 0x08,
            RENDER_CULL_CCW = 0x10,
            RENDER_DEPTH = 0x20
        }

        public static float[] m_DepthBuffer;

        public static vec3d Mat_MultiplyVector(mat4x4 m, vec3d i)
        {
            var v = new vec3d
            {
                x = i.x * m.m[0, 0] + i.y * m.m[1, 0] + i.z * m.m[2, 0] + i.w * m.m[3, 0],
                y = i.x * m.m[0, 1] + i.y * m.m[1, 1] + i.z * m.m[2, 1] + i.w * m.m[3, 1],
                z = i.x * m.m[0, 2] + i.y * m.m[1, 2] + i.z * m.m[2, 2] + i.w * m.m[3, 2],
                w = i.x * m.m[0, 3] + i.y * m.m[1, 3] + i.z * m.m[2, 3] + i.w * m.m[3, 3]
            };
            return v;
        }

        public static mat4x4 Mat_MakeIdentity()
        {
            var matrix = new mat4x4
            {
                m =
                {
                    [0, 0] = 1.0f,
                    [1, 1] = 1.0f,
                    [2, 2] = 1.0f,
                    [3, 3] = 1.0f
                }
            };
            return matrix;
        }

        public static mat4x4 Mat_MakeRotationX(float fAngleRad)
        {
            var matrix = new mat4x4
            {
                m =
                {
                    [0, 0] = 1.0f,
                    [1, 1] = cosf(fAngleRad),
                    [1, 2] = sinf(fAngleRad),
                    [2, 1] = -sinf(fAngleRad),
                    [2, 2] = cosf(fAngleRad),
                    [3, 3] = 1.0f
                }
            };
            return matrix;
        }

        public static mat4x4 Mat_MakeRotationY(float fAngleRad)
        {
            var matrix = new mat4x4
            {
                m =
                {
                    [0, 0] = cosf(fAngleRad),
                    [0, 2] = sinf(fAngleRad),
                    [2, 0] = -sinf(fAngleRad),
                    [1, 1] = 1.0f,
                    [2, 2] = cosf(fAngleRad),
                    [3, 3] = 1.0f
                }
            };
            return matrix;
        }

        public static mat4x4 Mat_MakeRotationZ(float fAngleRad)
        {
            var matrix = new mat4x4
            {
                m =
                {
                    [0, 0] = cosf(fAngleRad),
                    [0, 1] = sinf(fAngleRad),
                    [1, 0] = -sinf(fAngleRad),
                    [1, 1] = cosf(fAngleRad),
                    [2, 2] = 1.0f,
                    [3, 3] = 1.0f
                }
            };
            return matrix;
        }

        public static mat4x4 Mat_MakeScale(float x, float y, float z)
        {
            var matrix = new mat4x4
            {
                m =
                {
                    [0, 0] = x,
                    [1, 1] = y,
                    [2, 2] = z,
                    [3, 3] = 1.0f
                }
            };
            return matrix;
        }

        public static mat4x4 Mat_MakeTranslation(float x, float y, float z)
        {
            var matrix = new mat4x4
            {
                m =
                {
                    [0, 0] = 1.0f,
                    [1, 1] = 1.0f,
                    [2, 2] = 1.0f,
                    [3, 3] = 1.0f,
                    [3, 0] = x,
                    [3, 1] = y,
                    [3, 2] = z
                }
            };
            return matrix;
        }

        public static mat4x4 Mat_MakeProjection(float fFovDegrees, float fAspectRatio, float fNear, float fFar)
        {
            var fFovRad = 1.0f / tanf(fFovDegrees * 0.5f / 180.0f * 3.14159f);
            var matrix = new mat4x4
            {
                m =
                {
                    [0, 0] = fAspectRatio * fFovRad,
                    [1, 1] = fFovRad,
                    [2, 2] = fFar / (fFar - fNear),
                    [3, 2] = -fFar * fNear / (fFar - fNear),
                    [2, 3] = 1.0f,
                    [3, 3] = 0.0f
                }
            };
            return matrix;
        }

        public static mat4x4 Mat_MultiplyMatrix(mat4x4 m1, mat4x4 m2)
        {
            var matrix = new mat4x4();
            for (var c = 0; c < 4; c++)
            for (var r = 0; r < 4; r++)
                matrix.m[r, c] = m1.m[r, 0] * m2.m[0, c] + m1.m[r, 1] * m2.m[1, c] + m1.m[r, 2] * m2.m[2, c] +
                                 m1.m[r, 3] * m2.m[3, c];
            return matrix;
        }


        public static void DrawTriangleFlat(this Graphics2DGL gfx, triangle tri)
        {
            gfx.FillTriangle((int) tri.p[0].x, (int) tri.p[0].y, (int) tri.p[1].x, (int) tri.p[1].y, (int) tri.p[2].x,
                (int) tri.p[2].y, tri.col);
        }

        public static void DrawTriangleWire(this Graphics2DGL gfx, triangle tri, Pixel col)
        {
            gfx.DrawTriangle((int) tri.p[0].x, (int) tri.p[0].y, (int) tri.p[1].x, (int) tri.p[1].y, (int) tri.p[2].x,
                (int) tri.p[2].y, col);
        }

        public static void TexturedTriangle(this Graphics2DGL gfx, int x1, int y1, float u1, float v1, float w1,
            int x2, int y2, float u2, float v2, float w2,
            int x3, int y3, float u3, float v3, float w3, Sprite spr)
        {
            if (m_DepthBuffer == null)
                m_DepthBuffer = new float[gfx.DrawTarget.Width * gfx.DrawTarget.Height];

            if (y2 < y1)
            {
                swap(y1, y2);
                swap(x1, x2);
                swap(u1, u2);
                swap(v1, v2);
                swap(w1, w2);
            }

            if (y3 < y1)
            {
                swap(y1, y3);
                swap(x1, x3);
                swap(u1, u3);
                swap(v1, v3);
                swap(w1, w3);
            }

            if (y3 < y2)
            {
                swap(y2, y3);
                swap(x2, x3);
                swap(u2, u3);
                swap(v2, v3);
                swap(w2, w3);
            }

            var dy1 = y2 - y1;
            var dx1 = x2 - x1;
            var dv1 = v2 - v1;
            var du1 = u2 - u1;
            var dw1 = w2 - w1;

            var dy2 = y3 - y1;
            var dx2 = x3 - x1;
            var dv2 = v3 - v1;
            var du2 = u3 - u1;
            var dw2 = w3 - w1;

            float tex_u, tex_v, tex_w;

            float dax_step = 0,
                dbx_step = 0,
                du1_step = 0,
                dv1_step = 0,
                du2_step = 0,
                dv2_step = 0,
                dw1_step = 0,
                dw2_step = 0;

            if (dy1 > 0) dax_step = dx1 / (float) abs(dy1);
            if (dy2 > 0) dbx_step = dx2 / (float) abs(dy2);

            if (dy1 > 0) du1_step = du1 / (float) abs(dy1);
            if (dy1 > 0) dv1_step = dv1 / (float) abs(dy1);
            if (dy1 > 0) dw1_step = dw1 / (float) abs(dy1);

            if (dy2 > 0) du2_step = du2 / (float) abs(dy2);
            if (dy2 > 0) dv2_step = dv2 / (float) abs(dy2);
            if (dy2 > 0) dw2_step = dw2 / (float) abs(dy2);

            if (dy1 > 0)
                for (var i = y1; i <= y2; i++)
                {
                    var ax = (int) (x1 + (i - y1) * dax_step);
                    var bx = (int) (x1 + (i - y1) * dbx_step);

                    var tex_su = u1 + (i - y1) * du1_step;
                    var tex_sv = v1 + (i - y1) * dv1_step;
                    var tex_sw = w1 + (i - y1) * dw1_step;

                    var tex_eu = u1 + (i - y1) * du2_step;
                    var tex_ev = v1 + (i - y1) * dv2_step;
                    var tex_ew = w1 + (i - y1) * dw2_step;

                    if (ax > bx)
                    {
                        swap(ax, bx);
                        swap(tex_su, tex_eu);
                        swap(tex_sv, tex_ev);
                        swap(tex_sw, tex_ew);
                    }

                    tex_u = tex_su;
                    tex_v = tex_sv;
                    tex_w = tex_sw;

                    var tstep = 1.0f / (bx - ax);
                    var t = 0.0f;

                    for (var j = ax; j < bx; j++)
                    {
                        tex_u = (1.0f - t) * tex_su + t * tex_eu;
                        tex_v = (1.0f - t) * tex_sv + t * tex_ev;
                        tex_w = (1.0f - t) * tex_sw + t * tex_ew;

                        if (tex_w > m_DepthBuffer[i * gfx.DrawTarget.Width + j])
                        {
                            gfx.Draw(j, i, spr.Sample(tex_u / tex_w, tex_v / tex_w));
                            m_DepthBuffer[i * gfx.DrawTarget.Width + j] = tex_w;
                        }

                        t += tstep;
                    }
                }

            dy1 = y3 - y2;
            dx1 = x3 - x2;
            dv1 = v3 - v2;
            du1 = u3 - u2;
            dw1 = w3 - w2;

            if (dy1 > 0) dax_step = dx1 / (float) abs(dy1);
            if (dy2 > 0) dbx_step = dx2 / (float) abs(dy2);

            du1_step = 0;
            dv1_step = 0;
            if (dy1 > 0) du1_step = du1 / (float) abs(dy1);
            if (dy1 > 0) dv1_step = dv1 / (float) abs(dy1);
            if (dy1 > 0) dw1_step = dw1 / (float) abs(dy1);

            if (dy1 > 0)
                for (var i = y2; i <= y3; i++)
                {
                    var ax = (int) (x2 + (i - y2) * dax_step);
                    var bx = (int) (x1 + (i - y1) * dbx_step);

                    var tex_su = u2 + (i - y2) * du1_step;
                    var tex_sv = v2 + (i - y2) * dv1_step;
                    var tex_sw = w2 + (i - y2) * dw1_step;

                    var tex_eu = u1 + (i - y1) * du2_step;
                    var tex_ev = v1 + (i - y1) * dv2_step;
                    var tex_ew = w1 + (i - y1) * dw2_step;

                    if (ax > bx)
                    {
                        swap(ax, bx);
                        swap(tex_su, tex_eu);
                        swap(tex_sv, tex_ev);
                        swap(tex_sw, tex_ew);
                    }

                    tex_u = tex_su;
                    tex_v = tex_sv;
                    tex_w = tex_sw;

                    var tstep = 1.0f / (bx - ax);
                    var t = 0.0f;

                    for (var j = ax; j < bx; j++)
                    {
                        tex_u = (1.0f - t) * tex_su + t * tex_eu;
                        tex_v = (1.0f - t) * tex_sv + t * tex_ev;
                        tex_w = (1.0f - t) * tex_sw + t * tex_ew;

                        if (tex_w > m_DepthBuffer[i * gfx.DrawTarget.Width + j])
                        {
                            gfx.Draw(j, i, spr.Sample(tex_u / tex_w, tex_v / tex_w));
                            m_DepthBuffer[i * gfx.DrawTarget.Width + j] = tex_w;
                        }

                        t += tstep;
                    }
                }
        }

        public class vec2d
        {
            public float x;
            public float y;
            public float z;

            public static vec2d operator +(vec2d v1, vec2d v2)
            {
                return new vec2d
                {
                    //     w = v1.w + v2.w,
                    x = v1.x + v2.x,
                    y = v1.y + v2.y,
                    z = v1.z + v2.z
                };
            }

            public static vec2d operator +(vec2d v1, float val)
            {
                return new vec2d
                {
                    //   w = v1.w + val,
                    x = v1.x + val,
                    y = v1.y + val,
                    z = v1.z + val
                };
            }

            public static vec2d operator -(vec2d v1, vec2d v2)
            {
                return new vec2d
                {
                    // w = v1.w - v2.w,
                    x = v1.x - v2.x,
                    y = v1.y - v2.y,
                    z = v1.z - v2.z
                };
            }

            public static vec2d operator -(vec2d v1, float val)
            {
                return new vec2d
                {
                    //w = v1.w - val,
                    x = v1.x - val,
                    y = v1.y - val,
                    z = v1.z - val
                };
            }

            public static vec2d operator *(vec2d v1, vec2d v2)
            {
                return new vec2d
                {
                    //w = v1.w * v2.w,
                    x = v1.x * v2.x,
                    y = v1.y * v2.y,
                    z = v1.z * v2.z
                };
            }

            public static vec2d operator *(vec2d v1, float val)
            {
                return new vec2d
                {
                    //w = v1.w * val,
                    x = v1.x * val,
                    y = v1.y * val,
                    z = v1.z * val
                };
            }

            public static vec2d operator /(vec2d v1, vec2d v2)
            {
                return new vec2d
                {
                    //w = v1.w / v2.w,
                    x = v1.x / v2.x,
                    y = v1.y / v2.y,
                    z = v1.z / v2.z
                };
            }

            public static vec2d operator /(vec2d v1, float val)
            {
                return new vec2d
                {
                    //       w = v1.w / val,
                    x = v1.x / val,
                    y = v1.y / val,
                    z = v1.z / val
                };
            }
        }

        public class vec3d
        {
            public float w = 1F; // Need a 4th term to perform sensible matrix vector multiplication
            public float x;
            public float y;
            public float z;

            public static vec3d operator +(vec3d v1, vec3d v2)
            {
                return new vec3d
                {
                    w = v1.w + v2.w,
                    x = v1.x + v2.x,
                    y = v1.y + v2.y,
                    z = v1.z + v2.z
                };
            }

            public static vec3d operator +(vec3d v1, float val)
            {
                return new vec3d
                {
                    w = v1.w + val,
                    x = v1.x + val,
                    y = v1.y + val,
                    z = v1.z + val
                };
            }

            public static vec3d operator -(vec3d v1, vec3d v2)
            {
                return new vec3d
                {
                    w = v1.w - v2.w,
                    x = v1.x - v2.x,
                    y = v1.y - v2.y,
                    z = v1.z - v2.z
                };
            }

            public static vec3d operator -(vec3d v1, float val)
            {
                return new vec3d
                {
                    w = v1.w - val,
                    x = v1.x - val,
                    y = v1.y - val,
                    z = v1.z - val
                };
            }

            public static vec3d operator *(vec3d v1, vec3d v2)
            {
                return new vec3d
                {
                    w = v1.w * v2.w,
                    x = v1.x * v2.x,
                    y = v1.y * v2.y,
                    z = v1.z * v2.z
                };
            }

            public static vec3d operator *(vec3d v1, float val)
            {
                return new vec3d
                {
                    w = v1.w * val,
                    x = v1.x * val,
                    y = v1.y * val,
                    z = v1.z * val
                };
            }

            public static vec3d operator /(vec3d v1, vec3d v2)
            {
                return new vec3d
                {
                    w = v1.w / v2.w,
                    x = v1.x / v2.x,
                    y = v1.y / v2.y,
                    z = v1.z / v2.z
                };
            }

            public static vec3d operator /(vec3d v1, float val)
            {
                return new vec3d
                {
                    w = v1.w / val,
                    x = v1.x / val,
                    y = v1.y / val,
                    z = v1.z / val
                };
            }

            public static float DotProduct(vec3d v1, vec3d v2)
            {
                return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
            }

            public static float Length(vec3d v)
            {
                return sqrtf(DotProduct(v, v));
            }

            public static vec3d Normalize(vec3d v)
            {
                var l = Length(v);
                return new vec3d {x = v.x / l, y = v.y / l, z = v.z / l, w = v.w};
            }

            public static vec3d CrossProduct(vec3d v1, vec3d v2)
            {
                var v = new vec3d
                {
                    x = v1.y * v2.z - v1.z * v2.y,
                    y = v1.z * v2.x - v1.x * v2.z,
                    z = v1.x * v2.y - v1.y * v2.x
                };
                return v;
            }

            public static vec3d IntersectPlane(vec3d plane_p, vec3d plane_n, vec3d lineStart, vec3d lineEd, float t)
            {
                plane_n = Normalize(plane_n);
                var plane_d = -DotProduct(plane_n, plane_p);
                var ad = DotProduct(lineStart, plane_n);
                var bd = DotProduct(lineEd, plane_n);
                t = (-plane_d - ad) / (bd - ad);
                var lineStartToEnd = lineEd - lineStart;
                var lineToIntersect = lineStartToEnd * t;

                return lineStart + lineToIntersect;
            }
        }

        public class triangle
        {
            public Pixel col = Pixel.BLANK;
            public vec3d[] p = new vec3d[3];
            public vec2d[] t = new vec2d[3];

            public static int ClipAgainstPlane(vec3d plane_p, vec3d plane_n, triangle in_tri, triangle out_tri1,
                triangle out_tri2)
            {
                plane_n = vec3d.Normalize(plane_n);
                out_tri1.t[0] = in_tri.t[0];
                out_tri2.t[0] = in_tri.t[0];
                out_tri1.t[1] = in_tri.t[1];
                out_tri2.t[1] = in_tri.t[1];
                out_tri1.t[2] = in_tri.t[2];
                out_tri2.t[2] = in_tri.t[2];

                Func<vec3d, float> dist = delegate(vec3d p)
                {
                    var n = vec3d.Normalize(p);
                    return plane_n.x * p.x + plane_n.y * p.y + plane_n.z * p.z - vec3d.DotProduct(plane_n, plane_p);
                };

                var inside_points = new vec3d[3];
                var outside_points = new vec3d[3];
                var inside_tex = new vec2d[3];
                var outside_tex = new vec2d[3];
                var nInsidePointCount = 0;
                var nOutsidePointCount = 0;
                var nInsideTexCount = 0;
                var nOutsideTexCount = 0;

                var d0 = dist(in_tri.p[0]);
                var d1 = dist(in_tri.p[1]);
                var d2 = dist(in_tri.p[2]);

                if (d0 >= 0)
                {
                    inside_points[nInsidePointCount++] = in_tri.p[0];
                    inside_tex[nInsideTexCount++] = in_tri.t[0];
                }
                else
                {
                    outside_points[nOutsidePointCount++] = in_tri.p[0];
                    outside_tex[nOutsideTexCount++] = in_tri.t[0];
                }

                if (d1 >= 0)
                {
                    inside_points[nInsidePointCount++] = in_tri.p[1];
                    inside_tex[nInsideTexCount++] = in_tri.t[1];
                }
                else
                {
                    outside_points[nOutsidePointCount++] = in_tri.p[1];
                    outside_tex[nOutsideTexCount++] = in_tri.t[1];
                }

                if (d2 >= 0)
                {
                    inside_points[nInsidePointCount++] = in_tri.p[2];
                    inside_tex[nInsideTexCount++] = in_tri.t[2];
                }
                else
                {
                    outside_points[nOutsidePointCount++] = in_tri.p[2];
                    outside_tex[nOutsideTexCount++] = in_tri.t[2];
                }

                if (nInsidePointCount == 0)
                    return 0;

                if (nInsidePointCount == 3)
                {
                    out_tri1 = in_tri;
                    return 1;
                }

                if (nInsidePointCount == 1 && nOutsidePointCount == 2)
                {
                    out_tri1.col = Pixel.MAGENTA;
                    out_tri1.p[0] = inside_points[0];
                    out_tri1.t[0] = inside_tex[0];

                    float t = 0;
                    out_tri1.p[1] = vec3d.IntersectPlane(plane_p, plane_n, inside_points[0], outside_points[0], t);
                    out_tri1.t[1].x = t * (outside_tex[0].x - inside_tex[0].x) + inside_tex[0].x;
                    out_tri1.t[1].y = t * (outside_tex[0].y - inside_tex[0].y) + inside_tex[0].y;
                    out_tri1.t[1].z = t * (outside_tex[0].z - inside_tex[0].z) + inside_tex[0].z;

                    out_tri1.p[2] = vec3d.IntersectPlane(plane_p, plane_n, inside_points[0], outside_points[1], t);
                    out_tri1.t[2].x = t * (outside_tex[1].x - inside_tex[0].x) + inside_tex[0].x;
                    out_tri1.t[2].y = t * (outside_tex[1].y - inside_tex[0].y) + inside_tex[0].y;
                    out_tri1.t[2].z = t * (outside_tex[1].z - inside_tex[0].z) + inside_tex[0].z;

                    return 1;
                }

                if (nInsidePointCount == 2 && nOutsidePointCount == 1)
                {
                    out_tri1.col = Pixel.GREEN;
                    out_tri2.col = Pixel.RED;

                    out_tri1.p[0] = inside_points[0];
                    out_tri1.t[0] = inside_tex[0];

                    out_tri1.p[1] = inside_points[1];
                    out_tri1.t[1] = inside_tex[1];

                    float t = 0;
                    out_tri1.p[2] = vec3d.IntersectPlane(plane_p, plane_n, inside_points[0], outside_points[0], t);
                    out_tri1.t[2].x = t * (outside_tex[0].x - inside_tex[0].x) + inside_tex[0].x;
                    out_tri1.t[2].y = t * (outside_tex[0].y - inside_tex[0].y) + inside_tex[0].y;
                    out_tri1.t[2].z = t * (outside_tex[0].z - inside_tex[0].z) + inside_tex[0].z;

                    out_tri2.p[1] = inside_points[1];
                    out_tri2.t[1] = inside_tex[1];
                    out_tri2.p[0] = out_tri1.p[2];
                    out_tri2.t[0] = out_tri1.t[2];
                    out_tri2.p[2] = vec3d.IntersectPlane(plane_p, plane_n, inside_points[1], outside_points[0], t);
                    out_tri2.t[2].x = t * (outside_tex[0].x - inside_tex[1].x) + inside_tex[1].x;
                    out_tri2.t[2].y = t * (outside_tex[0].y - inside_tex[1].y) + inside_tex[1].y;
                    out_tri2.t[2].z = t * (outside_tex[0].z - inside_tex[1].z) + inside_tex[1].z;

                    return 2;
                }

                return 0;
            }
        }

        public class mat4x4
        {
            public float[,] m =
            {
                {0F, 0F, 0F, 0F}
            };
        }

        public class mesh
        {
            public List<triangle> tris = new List<triangle>();
        }

        #region Math Ext

        private static float cosf(float num)
        {
            return (float) Math.Cos(num);
        }

        private static float sinf(float num)
        {
            return (float) Math.Sin(num);
        }

        private static float tanf(float num)
        {
            return (float) Math.Tan(num);
        }

        private static float sqrtf(float num)
        {
            return (float) Math.Sqrt(num);
        }

        private static void swap<T>(T lhs, T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        private static double abs(float num)
        {
            return Math.Abs(num);
        }

        #endregion
    }
}