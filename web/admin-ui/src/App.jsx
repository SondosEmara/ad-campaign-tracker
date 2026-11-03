import { NavLink, Route, Routes } from "react-router-dom";
import { LayoutDashboard, Megaphone, Link2, MousePointerClick, Package, ShoppingBag, Store } from "lucide-react";
import { ToastProvider } from "./components.jsx";
import Dashboard from "./pages/Dashboard.jsx";
import Products from "./pages/Products.jsx";
import Campaigns from "./pages/Campaigns.jsx";
import Links from "./pages/Links.jsx";
import Simulator from "./pages/Simulator.jsx";
import Orders from "./pages/Orders.jsx";

const NAV = [
  ["/", "Dashboard", LayoutDashboard],
  ["/products", "Products", Package],
  ["/campaigns", "Campaigns", Megaphone],
  ["/links", "Short links", Link2],
  ["/orders", "Orders", ShoppingBag],
  ["/simulator", "Customer simulator", MousePointerClick],
];

export default function App() {
  return (
    <ToastProvider>
      <div className="shell">
        <aside className="side">
          <div className="brand"><Store size={22} /> ClickToCart</div>
          <nav>
            {NAV.map(([to, label, Icon]) => (
              <NavLink key={to} to={to} end={to === "/"} className="nav-item">
                <Icon size={18} /> {label}
              </NavLink>
            ))}
          </nav>
          <p className="side-note">Demo data lives in memory and resets when services restart.</p>
        </aside>
        <main className="main">
          <Routes>
            <Route path="/" element={<Dashboard />} />
            <Route path="/products" element={<Products />} />
            <Route path="/campaigns" element={<Campaigns />} />
            <Route path="/links" element={<Links />} />
            <Route path="/orders" element={<Orders />} />
            <Route path="/simulator" element={<Simulator />} />
          </Routes>
        </main>
      </div>
    </ToastProvider>
  );
}
