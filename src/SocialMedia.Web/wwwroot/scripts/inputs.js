const InitialiseInput = (group) => {
	const input = group.querySelector("input, textarea, select");
	if (!input) {
		return;
	}

	const handleDisplay = (value) => {
		if (value && value.length > 0) {
			group.classList.remove("empty");
		} else {
			group.classList.add("empty");
		}
	};

	input.addEventListener("keyup", (e) => handleDisplay(e.target.value));
	input.addEventListener("change", (e) => handleDisplay(e.target.value));
	handleDisplay(input.value);
};

const InitialiseInputs = () => {
	const inputGroups = document.querySelectorAll(".sm-input");
	const count = inputGroups.length;

	for (let i = 0; i < count; i++) {
		InitialiseInput(inputGroups[i]);
	}
};

export default InitialiseInputs;
