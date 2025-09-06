import { Link } from 'react-router-dom';

export default function RecipeList({ recipes = [], onDelete }) {
  if (!recipes.length) {
    return <p>No recipes yet. Click "Add Recipe" to create one.</p>;
  }

  return (
    <div className="recipe-list">
      {recipes.map(r => (
        <div key={r.id} className="recipe-card">
          <h3>
            <Link to={`/view/${r.id}`}>{r.title}</Link>
          </h3>
          <p className="muted">{r.description}</p>
          <div className="card-actions">
            <Link to={`/edit/${r.id}`} className="btn">Edit</Link>
            <button className="btn danger" onClick={() => onDelete(r.id)}>Delete</button>
            <Link to={`/view/${r.id}`} className="btn secondary">View</Link>
          </div>
        </div>
      ))}
    </div>
  );
}