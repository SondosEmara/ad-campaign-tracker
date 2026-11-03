export const money = (n) => `${Math.round(n ?? 0).toLocaleString("en-US")} EGP`;
export const number = (n) => (n ?? 0).toLocaleString("en-US");
export const percent = (n) => `${((n ?? 0) * 100).toFixed(1)}%`;
export const ratio = (n) => (n ?? 0).toFixed(1);
export const roasTone = (r) => (r >= 3 ? "good" : r < 1 ? "bad" : "mid");
