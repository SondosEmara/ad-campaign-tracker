import { useState } from "react";
import { Plus } from "lucide-react";
import { api } from "../api.js";
import { useAsync } from "../hooks.js";
import { Badge, Empty, ErrorBox, Loading, Modal, PageHeader, statusTone, useToast } from "../components.jsx";
import { money } from "../format.js";

const ACTIONS = {
  Draft: [["activate", "Activate"], ["end", "End"]],
  Active: [["pause", "Pause"], ["end", "End"]],
  Paused: [["activate", "Resume"], ["end", "End"]],
  Ended: [],
};

export default function Campaigns() {
  const { data, error, loading, reload } = useAsync(api.campaigns);
  const [creating, setCreating] = useState(false);
  const toast = useToast();
  if (loading && !data) return <Loading />;
  if (error) return <ErrorBox error={error} retry={reload} />;

  async function change(id, action) {
    try {
      await api.changeStatus(id, action);
      toast(`Campaign ${action}d`);
      reload();
    } catch (err) {
      toast(err.message, "bad");
    }
  }

  return (
    <>
      <PageHeader title="Campaigns" subtitle="Links can only be created under an Active campaign.">
        <button className="btn accent" onClick={() => setCreating(true)}><Plus size={16} /> New campaign</button>
      </PageHeader>
      <section className="card">
        {data.length === 0 ? <Empty>No campaigns yet.</Empty> : (
          <div className="table-wrap"><table>
            <thead><tr><th>Name</th><th>Platform</th><th className="n">Budget</th><th>Status</th><th /></tr></thead>
            <tbody>
              {data.map((c) => (
                <tr key={c.id}>
                  <td><strong>{c.name}</strong></td>
                  <td>{c.platform}</td>
                  <td className="n">{money(c.budget)}</td>
                  <td><Badge tone={statusTone[c.status]}>{c.status}</Badge></td>
                  <td className="row-actions">
                    {ACTIONS[c.status].map(([action, label]) => (
                      <button key={action} className="btn small ghost" onClick={() => change(c.id, action)}>{label}</button>
                    ))}
                  </td>
                </tr>
              ))}
            </tbody>
          </table></div>
        )}
      </section>
      {creating && <CreateModal onClose={() => setCreating(false)} onCreated={() => { setCreating(false); reload(); }} />}
    </>
  );
}

function CreateModal({ onClose, onCreated }) {
  const toast = useToast();
  const [form, setForm] = useState({ name: "", platform: "Facebook", budget: 5000 });
  const set = (key) => (e) => setForm({ ...form, [key]: e.target.value });

  async function submit(e) {
    e.preventDefault();
    try {
      await api.createCampaign({ ...form, budget: Number(form.budget) });
      toast("Campaign created as Draft");
      onCreated();
    } catch (err) {
      toast(err.message, "bad");
    }
  }

  return (
    <Modal title="New campaign" onClose={onClose}>
      <form onSubmit={submit}>
        <label>Name<input value={form.name} onChange={set("name")} required /></label>
        <label>Platform
          <select value={form.platform} onChange={set("platform")}>
            <option>Facebook</option><option>Instagram</option>
          </select>
        </label>
        <label>Budget (EGP)<input type="number" min="1" value={form.budget} onChange={set("budget")} required /></label>
        <button className="btn accent" type="submit">Create campaign</button>
      </form>
    </Modal>
  );
}
