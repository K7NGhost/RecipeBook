import { useNavigate, useParams } from 'react-router-dom';
import RecipeForm from '../components/RecipeForm';
import { findById, updateRecipe } from '../api/recipes';
import { useEffect, useState } from 'react';

export default function EditRecipe() {
  const { id } = useParams();
  const nav = useNavigate();
  const [existing, setExisting] = useState(null);
  const [notFound, setNotFound] = useState(false);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let mounted = true;
    (async () => {
      const r = await findById(id);
      if (!mounted) return;
      if (!r) { setNotFound(true); setLoading(false); return; }
      setExisting(r);
      setLoading(false);
    })();
    return () => { mounted = false; };
  }, [id]);

  if (loading) return <p>Loading…</p>;
  if (notFound) return <p>Recipe not found.</p>;

  async function handleSave(patch) {
    await updateRecipe(id, patch);
    nav(`/view/${id}`);
  }

  return (
    <div>
      <h2>Edit Recipe</h2>
      <RecipeForm initial={existing} onSave={handleSave} submitLabel="Save Changes" />
    </div>
  );
}