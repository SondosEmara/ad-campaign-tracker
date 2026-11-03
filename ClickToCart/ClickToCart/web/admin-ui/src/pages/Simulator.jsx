import { useState } from "react";
import { api } from "../api.js";
import { useAsync } from "../hooks.js";
import { Empty, ErrorBox, Loading, PageHeader, useToast } from "../components.jsx";

const newVisitor = () => `v-${crypto.randomUUID().slice(0, 8)}`;

export default function Simulator() {
  const { data: links, error, loading, reload } = useAsync(api.links);
  const [code, setCode] = useState("");
  const [visitor, setVisitor] = useState(newVisitor);
  const [log, setLog] = useState([]);
  const toast = useToast();
  if (loading && !links) return <Loading />;
  if (error) return <ErrorBox error={error} retry={reload} />;

  const live = links.filter((l) => l.isEnabled);
  const current = live.find((l) => l.code === code) ?? live[0];
  const note = (text) => setLog((items) => [text, ...items]);

  async function click(buy) {
    try {
      const hit = await api.simulateClick({ code: current.code, visitorId: visitor });
      note(`${visitor} clicked ${current.adName} and landed on ${new URL(hit.targetUrl).pathname}`);
      if (buy) await purchase();
    } catch (err) {
      toast(err.message, "bad");
    }
  }

  async function purchase() {
    const order = await api.createOrder({ productId: current.productId, quantity: 1, linkCode: current.code, visitorId: visitor });
    note(`${visitor} bought ${order.productName}. Order ${order.id} was sent for attribution.`);
  }

  return (
    <>
      <PageHeader title="Customer simulator" subtitle="Play the customer: click an ad, then buy or leave. Check the Dashboard afterwards." />
      {live.length === 0 ? <Empty>No enabled links. Create one on the Products page.</Empty> : (
        <section className="card sim">
          <label>Ad link
            <select value={current.code} onChange={(e) => setCode(e.target.value)}>
              {live.map((l) => <option key={l.code} value={l.code}>{l.adName} · {l.productName} ({l.code})</option>)}
            </select>
          </label>
          <label>Visitor
            <div className="inline">
              <input value={visitor} onChange={(e) => setVisitor(e.target.value)} />
              <button className="btn ghost" onClick={() => setVisitor(newVisitor())}>New visitor</button>
            </div>
          </label>
          <div className="actions">
            <button className="btn ghost" onClick={() => click(false)}>Click and only view</button>
            <button className="btn accent" onClick={() => click(true)}>Click and buy</button>
          </div>
        </section>
      )}
      <section className="card">
        <h2>What happened</h2>
        {log.length === 0 ? <Empty>Nothing yet.</Empty> : <ul className="log">{log.map((l, i) => <li key={i}>{l}</li>)}</ul>}
        <p className="muted small">The Worker processes events in the background, so numbers can lag a second.</p>
      </section>
    </>
  );
}
