import {
  argbFromHex,
  themeFromSourceColor,
} from "https://esm.sh/@material/material-color-utilities";

export function generateAndApplyTheme(seedColorHex, isDark) {
  try {
    console.log("🎨 [JS] Generating theme for:", seedColorHex, "Dark:", isDark);

    const source = argbFromHex(seedColorHex);
    const theme = themeFromSourceColor(source);

    const lightColors = theme.schemes.light.toJSON();
    const darkColors = theme.schemes.dark.toJSON();

    applyCssVariables(lightColors, darkColors, isDark);

    console.log("✅ [JS] Theme applied successfully");
    return true;
  } catch (error) {
    console.error("❌ [JS] Error generating theme:", error);
    return false;
  }
}

function applyCssVariables(lightColors, darkColors, isDark) {
  const root = document.documentElement;
  const colors = isDark ? darkColors : lightColors;

  for (const [key, value] of Object.entries(colors)) {
    const cssVar = key.replace(/([A-Z])/g, "-$1").toLowerCase();
    const hexValue = argbToHex(value);
    root.style.setProperty(`--color-${cssVar}`, hexValue);
  }
}

function argbToHex(argb) {
  const r = (argb >> 16) & 255;
  const g = (argb >> 8) & 255;
  const b = argb & 255;
  return `#${((1 << 24) + (r << 16) + (g << 8) + b).toString(16).slice(1)}`;
}
