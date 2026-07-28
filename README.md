# Planar Reflection (Unity URP)

Unity URP에서 **Planar Reflection**을 직접 구현한 프로젝트입니다.

SSR(Screen Space Reflection)은 화면에 보이는 정보만 반사할 수 있어 화면 밖의 오브젝트나 가려진 물체는 정확하게 표현하지 못합니다.
공연장 프로젝트에서는 넓은 바닥을 정확하게 반사하는 것이 중요했기 때문에 Planar Reflection을 직접 구현하고 적용했습니다.

Planar Reflection 방식은 카메라를 만들어 허상에도 렌더링하는 방법입니다. 허상 카메라라고 부르겠습니다.
허상 카메라에서 렌더링한 Render Texture를 전역 셰이더 변수로 등록하고 사용하겠습니다.

---

## Demo

<p align="center">
  <img src="./Images/PlanarReflection.gif" width="700"/>
</p>

---

## Tech Stack

- Universal Render Pipeline (URP)
- C#
- HLSL

---

## Features

반사 평면의 법선을 기준으로 메인 카메라의 위치를 대칭 이동시키고,
시선(Forward)과 Up 벡터를 반사하여 허상 Camera를 생성합니다.

```cpp
Vector3 proj = normal * Vector3.Dot(normal, cam.transform.position - transform.position);
_reflectionCam.transform.position = cam.transform.position - 2 * proj;

Vector3 illusionForward = Vector3.Reflect(cam.transform.forward, normal);
Vector3 illusionUp = Vector3.Reflect(cam.transform.up, normal);
_reflectionCam.transform.LookAt(_reflectionCam.transform.position + illusionForward, illusionUp);
```


CalculateObliqueMatrix를 이용해 Reflection Camera의 Near Plane을 반사 평면에 맞게 기울여, 평면 뒤쪽 Geometry를 Clipping합니다.


```cpp
Matrix4x4 viewMat = _reflectionCam.worldToCameraMatrix;
Vector3 viewPosition = viewMat.MultiplyPoint(transform.position);
Vector3 viewNormal = viewMat.MultiplyVector(normal);
Vector4 plane = new Vector4(viewNormal.x, viewNormal.y, viewNormal.z, -Vector3.Dot(viewPosition, viewNormal));
_reflectionCam.projectionMatrix = _reflectionCam.CalculateObliqueMatrix(plane);
```

---

## Reference
- Youtube Planar Reflections for Unity's Built-in Render Pipeline! (Rafael Bordoni)
