# Planar Reflection (Unity URP)

Unity URP에서 **Planar Reflection**을 직접 구현한 프로젝트입니다.

SSR(Screen Space Reflection)은 화면에 보이는 정보만 반사할 수 있어 화면 밖의 오브젝트나 가려진 물체는 정확하게 표현하지 못합니다.

공연장 프로젝트에서는 넓은 바닥을 정확하게 반사하는 것이 중요했기 때문에 Planar Reflection을 직접 구현하고 적용했습니다.

---

## Demo

<p align="center">
  <img src="./Images/PlanarReflection.gif" width="700"/>
</p>

---

## Tech Stack

- Unity 6
- Universal Render Pipeline (URP)
- C#
- HLSL

---

## Features

### Reflection Camera

- Reflection Camera를 생성하여 장면을 한 번 더 렌더링
- Reflection Matrix를 이용하여 카메라를 반사면 기준으로 대칭 이동
- Oblique Projection을 적용하여 반사면 아래 영역 제거
- RenderTexture를 생성하여 반사 결과 출력

---

### Reflection Matrix

반사면의 Normal을 기준으로 Reflection Matrix를 생성하여
카메라를 반사 위치로 이동합니다.
