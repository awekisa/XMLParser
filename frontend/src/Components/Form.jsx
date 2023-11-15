import React from 'react';
import { useForm } from 'react-hook-form';
import { Button, Input, Flex } from '@chakra-ui/react';

function Form({ setResponseMessage }) {
	const { register, handleSubmit } = useForm();

	const onSubmit = async (data) => {
		const formData = new FormData();
		formData.append('file', data.file[0]);
		formData.append('fileName', removeFileExtension(data.file[0]?.name));

		const res = await fetch('https://localhost:7217/document', {
			method: 'POST',
			body: formData,
		})
			.then((res) => res.json())
			.then((data) => setResponseMessage(data));
	};

	return (
		<div className='App'>
			<form onSubmit={handleSubmit(onSubmit)}>
				<Flex>
					<Input
						type='file'
						{...register('file')}
					/>
					<Button
						colorScheme='teal'
						type='submit'
					>
						Submit
					</Button>
				</Flex>
			</form>
		</div>
	);
}

export default Form;

var removeFileExtension = function (fileName) {
	return fileName?.split('.').slice(0, -1).join('.');
};
