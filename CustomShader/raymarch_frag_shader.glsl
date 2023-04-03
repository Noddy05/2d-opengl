#version 400 core

in vec2 vCoordinates;

uniform vec4 color;
uniform float scaleRatio;
uniform float t;
const int MAX_POSITIONS = 16;
uniform vec3 positions[MAX_POSITIONS];
uniform vec3 sizes[MAX_POSITIONS];
uniform mat4 rotationMatrix = mat4(1);
uniform vec3 cameraPosition;
uniform int lookingAtIndex;

const float minStep = 0.0001;
const float reflectivity = 0.2;
const float reflectivityDamping = 2;
const float rayTraceReflectivity = 0.3;

vec3 lightVector = vec3(0, 1, 0);

out vec4 color_out;

float distanceToBox(vec3 fromRay, vec3 size){
    return length(max(abs(fromRay) - size, 0));
}
float distanceToMandelbulb(vec3 rayPosition){
    vec3 z = rayPosition;
    float power = (sin(t) + 1) + 2;
    float dr = 1;
    float r;

    for(int i = 0; i < 15; i++){
        r = length(z);
        if(r > 2)
            break;

        float theta = acos(z.z / r) * power;
        float phi = atan(z.y, z.x) * power;
        float zr = pow(r, power);
        dr = pow(r, power - 1) * power * dr + 1;

        z = zr * vec3(sin(theta) * cos(phi), sin(phi) * sin(theta), cos(theta));
        z += rayPosition;
    }


    return 0.5 * log(r) * r / dr;
}
vec3 GetNormal(vec3 rayPosition, int index){
    vec2 e = vec2(minStep, 0);
    float d = distanceToBox(rayPosition - positions[index], sizes[index]);
    vec3 n = vec3(
        d - distanceToBox(rayPosition - e.xyy - positions[index], sizes[index]),
        d - distanceToBox(rayPosition - e.yxy - positions[index], sizes[index]),
        d - distanceToBox(rayPosition - e.yyx - positions[index], sizes[index])
    );

    return normalize(n);
}
vec3 GetNormalMandelbulb(vec3 rayPosition){
    vec2 e = vec2(minStep, 0);
    float d = distanceToMandelbulb(rayPosition);
    vec3 n = vec3(
        d - distanceToMandelbulb(rayPosition - e.xyy),
        d - distanceToMandelbulb(rayPosition - e.yxy),
        d - distanceToMandelbulb(rayPosition - e.yyx)
    );

    return normalize(n);
}

float calculateShadow(vec3 rayPosition, int ignoreIndex){
    float distanceTravelled = 0;
    vec3 rayVec = lightVector;
    vec3 pos = rayPosition;

    while(distanceTravelled < 100){
        float minStepDistance = 10000;//distanceToMandelbulb(rayPosition);
        
        int closestIndex = 0;
        for(int j = 0; j < MAX_POSITIONS; j++){
            if(j == ignoreIndex)
                continue;
            float dist = distanceToBox(pos - positions[j], sizes[j]);
            if(minStepDistance >= dist){
                minStepDistance = dist;
                closestIndex = j;
            }
        }
        distanceTravelled += minStepDistance;
        if(minStepDistance < minStep){
            return 0.1;
        }
        pos = rayPosition + rayVec * distanceTravelled;

    }

    return 1;
}

float SDF(vec3 position, int ignoreIndex, out int closestIndex){
    float minStepDistance = 10000;
    int minIndex = 0;
    for(int i = 0; i < MAX_POSITIONS; i++){
        if(i == ignoreIndex)
            continue;

        float dist = distanceToBox(position - positions[i], sizes[i]);
        if(minStepDistance >= dist){
            minStepDistance = dist;
            minIndex = i;
        }
    }

    closestIndex = minIndex;
    return minStepDistance;
}

float calculateOcclusion(vec3 rayPosition, vec3 normal){
    float occlusion = 0;
    float e = 0.1;

    int ignoreVariable;
    for(int i = 1; i <= 5; i++){
        float d = e * float(i);
        occlusion += 1.0 - (d - SDF(rayPosition + d * normal, -1, ignoreVariable));
    }
    occlusion /= 5.0;

    return occlusion;
}

int calculateReflectedIndex(vec3 rayPosition, vec3 rayDirection, vec3 normal, int ignoreIndex){
    vec3 ray = normalize(reflect(rayDirection, normal));
    vec3 rayPos = rayPosition;
    float distanceTravelled = 0;
    for(int i = 0; i <= 100; i++){
        int closestIndex;
        float minStepDistance = SDF(rayPos, ignoreIndex, closestIndex);
        distanceTravelled += minStepDistance;
        if(minStepDistance < minStep){
            return closestIndex;
        }
        rayPos = rayPosition + ray * distanceTravelled;
    }

    return -1;
}


void main()
{
    lightVector = vec3(cos(t / 2), 1, sin(t / 2)) / 1.414;

    float x = (vCoordinates.x - 0.5) * scaleRatio;
    float y = vCoordinates.y - 0.5;
    vec3 rayVec = (rotationMatrix * vec4(normalize(vec3(x, y, 1)), 0)).xyz;
    vec3 rayStart = cameraPosition;
    vec3 rayPosition = rayStart;

    float distanceTravelled = 0;
    while(distanceTravelled < 1000){
        int closestIndex;
        float minStepDistance = SDF(rayPosition, -1, closestIndex);
        distanceTravelled += minStepDistance;

        if(minStepDistance < minStep){
            vec3 normal;
            if(closestIndex == -1)
                normal = GetNormalMandelbulb(rayPosition);
            else
                normal = GetNormal(rayPosition, closestIndex);
            float dotProduct = dot(normal, lightVector);
            dotProduct = min(max(dotProduct, 0), 1);
            float lightValue = calculateShadow(rayPosition, closestIndex);
            float ambientOcclusion = calculateOcclusion(rayPosition, normal);
            vec3 lightColor = vec3(1, 0.96, 0.86);
            vec3 reflectedLightRay = reflect(-lightVector, normal);
            float reflectionDotProduct = dot(normalize(rayStart - rayPosition), normalize(reflectedLightRay));
            float specularity = max(reflectionDotProduct, 0.0);
            float dampedSpecularity = pow(specularity, reflectivityDamping) * reflectivity;
            
            
            vec3 baseColor = vec3(1);
            switch(closestIndex){
                case 0:
                    baseColor = vec3(0.8, 0.2, 0.2);
                    break;
                case 1:
                    baseColor = vec3(0.2, 0.8, 0.2);
                    break;
                case 2:
                    baseColor = vec3(0.2, 0.2, 0.8);
                    break;
                default:
                    baseColor = vec3(1, 1, 1);
                    break;
            }
            if(lookingAtIndex == closestIndex){
                lightValue += 0.2;
                ambientOcclusion += 0.2;
            }
            vec3 reflectedColor = vec3(1);
            int reflectedIndex = calculateReflectedIndex(rayPosition, rayVec, normal, closestIndex);
            if(reflectedIndex != -1){
                switch(reflectedIndex){
                    case 0:
                        reflectedColor = vec3(0.8, 0.2, 0.2);
                        break;
                    case 1:
                        reflectedColor = vec3(0.2, 0.8, 0.2);
                        break;
                    case 2:
                        reflectedColor = vec3(0.2, 0.2, 0.8);
                        break;
                    default:
                        reflectedColor = vec3(1, 1, 1);
                        break;
                }
            }

            color_out = vec4((baseColor * (1 - rayTraceReflectivity) + reflectedColor * rayTraceReflectivity) 
                * dotProduct * lightValue * ambientOcclusion * lightColor + dampedSpecularity, 1);
            return;
        }
        rayPosition = rayStart + rayVec * distanceTravelled;
    }
    vec4 c = vec4(1.0, 1.0, 1.0, 1.0);

    color_out = color / 1000.0 + c;
}