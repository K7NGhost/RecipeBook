import { useParams, Link } from 'react-router-dom';
import { findById } from '../api/recipes';
import { useEffect, useState } from 'react';

export default function ViewRecipe() {
    const { id } = useParams();
    const [recipe, setRecipe] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        let mounted = true;
        (async () => {
            const r = await findById(id);
            if (mounted) setRecipe(r);
            setLoading(false);
        })();
        return () => { mounted = false; };
    }, [id]);

    if (loading) return <p>Loading...</p>;
    if (!recipe) return <p>Recipe not found.</p>

  return (
      <div>
        <h2>{recipe.title}</h2>
        <p className="muted">{recipe.description}</p>

        <h3>Ingredients</h3>
        <ul>
            {(recipe.ingredients || []).map((ing, i) => <li key={i}>{typeof ing === 'string' ? ing : (ing.ingredient ?? JSON.stringify(ing))}</li>)}
        </ul>

        <h3>Instructions</h3>
        <pre className="instructions">{Array.isArray(recipe.instructions) ? recipe.instructions.join('\n') : (recipe.instructions || '')}</pre>

        <p className="muted">Created: {new Date(recipe.createdDate).toLocaleString()}</p>

        <div className="card-actions">
          <Link to={`/edit/${recipe.id}`} className="btn">Edit</Link>
          <Link to="/" className="btn secondary">Back</Link>
        </div>
      </div>
  );
}