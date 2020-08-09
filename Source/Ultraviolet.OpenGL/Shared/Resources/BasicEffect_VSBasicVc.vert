﻿#includeres "Ultraviolet.OpenGL.Resources.BasicEffectPreamble.glsl" executing

 in vec4 uv_Position0;
 in vec4 uv_Color0;

out vec4 vDiffuse;
out vec4 vSpecular;
out vec4 vPositionPS;

void main()
{
	CommonVSOutput cout = ComputeCommonVSOutput(uv_Position0);
	SetCommonVSOutputParams;

	vDiffuse *= ConvertColor(uv_Color0);
}