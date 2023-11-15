import React, { useState } from 'react';
import { useForm } from 'react-hook-form';
import { Box, Button, Input, Flex, FormControl } from '@chakra-ui/react';

function Form({ setResponseMessage, setError }) {
	const { register, handleSubmit } = useForm();
	const [file, setFile] = useState();

	const handleFileChange = (evt) => {
		setFile(evt.target.files[0]);
	};
	const onSubmit = async (data) => {
		try {
			const formData = new FormData();
			formData.append('file', data.file[0]);
			formData.append('fileName', removeFileExtension(data.file[0]?.name));

			await fetch('https://localhost:7217/document', {
				method: 'POST',
				body: formData,
			})
				.then((res) => res.json())
				.then((data) => {
					setResponseMessage(data);
					setError('');
				});
		} catch (err) {
			setError(err.message);
		}
	};

	return (
		<Box>
			<form onSubmit={handleSubmit(onSubmit)}>
				<Flex>
					<FormControl onChange={handleFileChange}>
						<Input
							type='file'
							{...register('file')}
						/>
					</FormControl>
					<Button
						isDisabled={!file}
						colorScheme='teal'
						type='submit'
					>
						Submit
					</Button>
				</Flex>
			</form>
		</Box>
	);
}

export default Form;

var removeFileExtension = function (fileName) {
	return fileName?.split('.').slice(0, -1).join('.');
};
