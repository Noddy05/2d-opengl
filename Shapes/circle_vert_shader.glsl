#version 400 core

layout (location = 0) in vec2 vertexPosition;

uniform vec4 aColor;
uniform mat4 transform = mat4(1);

out vec2 vCoordinates;

void main()
{
    vCoordinates = vertexPosition.xy;
    gl_Position = transform * (vec4(vertexPosition, 0.0, 1.0));
}