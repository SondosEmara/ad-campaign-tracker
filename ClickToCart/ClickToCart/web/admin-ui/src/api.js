const env = import.meta.env;
const urls = {
  products: env.VITE_PRODUCTS_URL ?? "http://localhost:5101",
  orders: env.VITE_ORDERS_URL ?? "http://localhost:5102",
  marketing: env.VITE_MARKETING_URL ?? "http://localhost:5103",
};

async function readError(res) {
  try {
    const body = await res.json();
    return body.title ?? res.statusText;
  } catch {
    return res.statusText;
  }
}

async function request(base, path, { method = "GET", body } = {}) {
  const res = await fetch(base + path, {
    method,
    headers: { "Content-Type": "application/json" },
    body: body ? JSON.stringify(body) : undefined,
  });
  if (!res.ok) throw new Error(await readError(res));
  return res.json();
}

export const api = {
  products: () => request(urls.products, "/api/products"),
  orders: () => request(urls.orders, "/api/orders"),
  createOrder: (body) => request(urls.orders, "/api/orders", { method: "POST", body }),
  campaigns: () => request(urls.marketing, "/api/campaigns"),
  createCampaign: (body) => request(urls.marketing, "/api/campaigns", { method: "POST", body }),
  changeStatus: (id, action) => request(urls.marketing, `/api/campaigns/${id}/status/${action}`, { method: "POST" }),
  links: () => request(urls.marketing, "/api/links"),
  createLink: (body) => request(urls.marketing, "/api/links", { method: "POST", body }),
  disableLink: (code) => request(urls.marketing, `/api/links/${code}/disable`, { method: "POST" }),
  dashboard: () => request(urls.marketing, "/api/dashboard"),
  simulateClick: (body) => request(urls.marketing, "/api/simulator/click", { method: "POST", body }),
};
